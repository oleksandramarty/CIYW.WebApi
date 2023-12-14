using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediatr.Auth.Queries;
using CIYW.Mediatr.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediatr.Auth.Handlers;

public class SignInCommandHandler: IRequestHandler<SignInCommand>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IEntityValidator _entityValidator;
    private readonly IAuthRepository _authRepository;
    private readonly UserManager<User> _userManager;

    public SignInCommandHandler(
        IMapper mapper, 
        IMediator mediator, 
        IGenericRepository<User> userRepository, 
        IEntityValidator entityValidator, 
        IAuthRepository authRepository, 
        UserManager<User> userManager)
    {
        _mapper = mapper;
        _mediator = mediator;
        _userRepository = userRepository;
        _entityValidator = entityValidator;
        _authRepository = authRepository;
        _userManager = userManager;
    }

    public async Task Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        if (!command.Email.TrimWhiteSpaces().Equals(command.ConfirmEmail.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.EmailsDoesntMatch, 409, null, EntityTypeEnum.User.ToString());
        }
        
        if (!command.Password.TrimWhiteSpaces().Equals(command.ConfirmPassword.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.PasswordsDoesntMatch, 409, null, EntityTypeEnum.User.ToString());
        }
        
        if (!command.IsAgree)
        {
            throw new LoggerException(ErrorMessages.AgreeBeforeSignIn, 409, null, EntityTypeEnum.User.ToString());
        }
        
        await _entityValidator.ValidateExistParamAsync<User>(u => u.Email == command.Email, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Email), cancellationToken);
        await _entityValidator.ValidateExistParamAsync<User>(u => u.PhoneNumber == command.Phone, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Phone), cancellationToken);
        await _entityValidator.ValidateExistParamAsync<User>(u => u.Login == command.Login, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Login), cancellationToken);
        
        User user = this._mapper.Map<SignInCommand, User>(command);
      
        var res = await _userManager.CreateAsync(user, command.Password);

        if (res.Succeeded)
        {
            res = await _userManager.AddToRoleAsync(user, RoleProvider.User);
        }

        List<IdentityUserLogin<Guid>> logins = this.CreateUserLogins(user);

        await this._authRepository.UpdateUserLoginsAsync(user.Id, logins, cancellationToken);

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
}
