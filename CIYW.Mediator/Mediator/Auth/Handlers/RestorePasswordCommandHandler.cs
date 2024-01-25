using System.Text.RegularExpressions;
using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Models.Common;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class RestorePasswordCommandHandler: UserEntityValidatorHelper, IRequestHandler<RestorePasswordCommand>
{
    private readonly UserManager<User> userManager;
    private readonly IAuthRepository authRepository;
    private readonly IGenericRepository<User> userRepository;
    private readonly IGenericRepository<RestorePassword> restorePasswordRepository;
    
    public RestorePasswordCommandHandler(
        UserManager<User> userManager,
        IAuthRepository authRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<RestorePassword> restorePasswordRepository,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.authRepository = authRepository;
        this.userManager = userManager;
        this.userRepository = userRepository;
        this.restorePasswordRepository = restorePasswordRepository;
    }
    
    public async Task Handle(RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        this.ValidateRequest<RestorePasswordCommand>(command, () => new RestorePasswordCommandValidator());
        
        var dateNow = DateTime.UtcNow;
        Regex rgx = new Regex("[^a-zA-Z0-9]");

        IList<RestorePassword> restores = await this.restorePasswordRepository.GetListByPropertyAsync(r =>
            r.Created >= dateNow.AddHours(-4) &&
            r.Created <= dateNow.AddHours(4), cancellationToken);

        if (!restores.Any())
        {
            throw new LoggerException(ErrorMessages.NotFound, 404);
        }

        RestorePassword restore = null;
        string normalizedUrl = command.Url.ToUpper();
        
        foreach (var item in restores)
        {
            if (rgx.Replace($"{item.Id}{item.UserId}{item.Url}", "").ToUpper().Equals(normalizedUrl))
            {
                restore = item;
            }
        }

        if (restore.Used.HasValue)
        {
            throw new LoggerException(ErrorMessages.AlreadyUsed, 409);
        }
        
        this.ValidateExist<RestorePassword, Guid?>(restore, restore?.Id);
        
        User user = await this.userRepository.GetByPropertyAsync(u =>
            u.Login == command.Login &&
            u.Email == command.Email &&
            u.PhoneNumber == command.Phone && 
            u.Id == restore.UserId, cancellationToken);

        this.ValidateExist<User, Guid?>(user, user?.Id);
        
        await this.authRepository.LogOutUserAsync(user.Id, cancellationToken);
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, command.Password);
        
        if (!result.Succeeded)
        {
            throw new LoggerException(ErrorMessages.AuthError, 409, user.Id, result.CreateFields());
        }
        
        restore.Used = dateNow;
        user.Restored = dateNow;
        user.IsTemporaryPassword = true;
        
        await this.userRepository.UpdateAsync(user, cancellationToken);
    }
}