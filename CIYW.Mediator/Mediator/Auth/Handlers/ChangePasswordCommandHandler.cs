using AutoMapper;
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

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class ChangePasswordCommandHandler: UserEntityValidatorHelper, IRequestHandler<ChangePasswordCommand>
{
    private readonly UserManager<User> userManager;
    private readonly IAuthRepository authRepository;
    private readonly IGenericRepository<User> userRepository;
    private readonly IGenericRepository<RestorePassword> restorePasswordRepository;
    
    public ChangePasswordCommandHandler(
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
    
    public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        this.ValidateRequest<ChangePasswordCommand>(command, () => new ChangePasswordCommandValidator());

        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        User user = await this.userRepository.GetByIdAsync(userId, cancellationToken);
        
        this.ValidateExist<User, Guid?>(user, user?.Id);
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, command.NewPassword);
        
        if (!result.Succeeded)
        {
            throw new LoggerException(result.GetIdentityErrors(), 409, user.Id);
        }
        
        await this.userRepository.UpdateAsync(user, cancellationToken);
    }
}