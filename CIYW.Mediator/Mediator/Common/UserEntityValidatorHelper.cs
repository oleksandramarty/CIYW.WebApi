using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using FluentValidation;
using MediatR;

namespace CIYW.Mediator.Mediator.Common;

public class UserEntityValidatorHelper
{
    private readonly IEntityValidator entityValidator;
    private readonly ICurrentUserProvider currentUserProvider;

    public UserEntityValidatorHelper(
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider)
    {
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    protected void ValidateRequest<TCommand, TResult>(TCommand command, Func<IValidator<TCommand>> validatorFactory) where TCommand : IRequest<TResult>
    {
        this.entityValidator.ValidateRequest<TCommand, TResult>(command, validatorFactory); 
    }
    
    protected async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken)
    {
        return await this.currentUserProvider.GetUserIdAsync(cancellationToken);
    }
    
    protected async Task IsUserInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        await this.currentUserProvider.IsUserInRoleAsync(roleName, cancellationToken);
    }
    
    protected async Task IsUserUserAsync(CancellationToken cancellationToken)
    {
        await this.IsUserInRoleAsync(RoleProvider.User, cancellationToken);
    }
    
    protected async Task IsUserAdminAsync(CancellationToken cancellationToken)
    {
        await this.IsUserInRoleAsync(RoleProvider.Admin, cancellationToken);
    }
    
    protected async Task<bool> HasUserInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        return await this.currentUserProvider.HasUserInRoleAsync(roleName, cancellationToken);
    }
    
    protected async Task<bool> HasUserUserAsync(CancellationToken cancellationToken)
    {
        return await this.HasUserInRoleAsync(RoleProvider.User, cancellationToken);
    }
    
    protected async Task<bool> HasUserAdminAsync(CancellationToken cancellationToken)
    {
        return await this.HasUserInRoleAsync(RoleProvider.Admin, cancellationToken);
    }

    protected void ValidateExist<T, TId>(T entity, TId? entityId) where T : class
    {
        this.entityValidator.ValidateExist<T, TId>(entity, entityId);
    }
    
    protected void HasAccess<TEntity>(TEntity entity, Guid userId)
    {
        this.entityValidator.HasAccess<TEntity>(entity, userId);
    }
}