using CIYW.Const.Const;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediatr.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediatr.Auth.Handlers;

public class SignInCommandHandler: IRequestHandler<SignInCommand>
{
    private readonly IGenericService<User> _userService;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public SignInCommandHandler(
        IGenericService<User> userService, IAuthService authService, IMediator mediator, UserManager<User> userManager)
    {
        _userService = userService;
        _authService = authService;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task Handle(SignInCommand command, CancellationToken cancellationToken)
    {
      // TODO validate command in advance
      await this.ValidateUserParamAsync(command.Email, DefaultConst.Email, cancellationToken);
      await this.ValidateUserParamAsync(command.Phone, DefaultConst.Phone, cancellationToken);
      await this.ValidateUserParamAsync(command.Login, DefaultConst.Login, cancellationToken);

      User user = this.CreateUser(command);
      
      var res = await _userManager.CreateAsync(user, command.Password);

      if (res.Succeeded)
      {
          command.Role = RoleProvider.User;
          if (res.Succeeded)
          {
              res = await _userManager.AddToRoleAsync(user, command.Role);
          }
      }

      List<IdentityUserLogin<Guid>> logins = this.CreateUserLogins(user);

      await this._authService.UpdateUserLoginsAsync(user.Id, logins, cancellationToken);

      await this._mediator.Send(new AuthLogoutQuery(user.Id), cancellationToken);
    }

    private List<IdentityUserLogin<Guid>> CreateUserLogins(User user)
    {
        return new List<IdentityUserLogin<Guid>>
        {
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWLogin,
                ProviderKey = user.Login,
                ProviderDisplayName = user.Login
            },
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWEmail,
                ProviderKey = user.Email,
                ProviderDisplayName = user.Email
            },
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWPhone,
                ProviderKey = user.PhoneNumber,
                ProviderDisplayName = user.PhoneNumber
            }
        };
    }

    private User CreateUser(SignInCommand command)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Patronymic = command.Patronymic,
            PhoneNumber = command.Phone,
            Email = command.Email,
            Created = DateTime.UtcNow,
            IsBlocked = false,
            UserName = command.Phone,
            Login = command.Login,
            IsTemporaryPassword = true,
        };
    }

    private async Task ValidateUserParamAsync(string value, string param, CancellationToken cancellationToken)
    {
        User user = null;
        switch (param)
        {
            case DefaultConst.Email:
                user = await this._userService.GetByPropertyAsync(u => u.Email == value, cancellationToken);
                break;
            case DefaultConst.Phone:
                user = await this._userService.GetByPropertyAsync(u => u.PhoneNumber == value, cancellationToken);
                break;
            case DefaultConst.Login:
                user = await this._userService.GetByPropertyAsync(u => u.Login == value, cancellationToken);
                break;
        }
        
        if (user != null)
        {
            throw new LoggerException(string.Format(ErrorMessages.UserWithParamExist, param), 409, null, EntityTypeEnum.User.ToString());
        }
    }
}