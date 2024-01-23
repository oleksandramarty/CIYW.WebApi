using System.Linq.Expressions;
using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Users.Requests;
using FluentValidation;
using MediatR;

namespace CIYW.Mediator.Mediator.Common;

public class UserEntityValidatorHelper
{
    private IMapper mapper;
    private readonly IEntityValidator entityValidator;
    private readonly ICurrentUserProvider currentUserProvider;

    public UserEntityValidatorHelper(
        IMapper mapper,
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider)
    {
        this.mapper = mapper;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    protected async Task ValidateExistParamAsync<T>(Expression<Func<T, bool>> predicate, string customErrorMessage, CancellationToken cancellationToken) where T : class
    {
        await this.entityValidator.ValidateExistParamAsync<T>(predicate, customErrorMessage, cancellationToken); 
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
    
    protected async Task HasAccess<TEntity>(TEntity entity, Guid userId, CancellationToken cancellationToken, string fieldName = "UserId")
    {
        bool isAdmin = await this.HasUserInRoleAsync(RoleProvider.Admin, cancellationToken);
        
        if (isAdmin)
        {
            return;
        }
        
        this.entityValidator.HasAccess<TEntity>(entity, userId, fieldName);
    }

    protected MappedHelperResponse<TMapped, T> GetMappedHelper<TMapped, T>(T entity)
    {
        return new MappedHelperResponse<TMapped, T>(mapper.Map<T, TMapped>(entity), entity);
    }

    protected async Task CheckUserCommandAsync(Guid? userId, CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!command.Email.TrimWhiteSpaces().Equals(command.ConfirmEmail.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.EmailsDoesntMatch, 409);
        }
        
        if (!command.Password.TrimWhiteSpaces().Equals(command.ConfirmPassword.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.PasswordsDoesntMatch, 409);
        }
        
        if (!command.IsAgree)
        {
            throw new LoggerException(ErrorMessages.AgreeBeforeSignIn, 409);
        }
        
        await this.ValidateExistParamAsync<User>(u => (userId.HasValue && u.Id != userId.Value || !userId.HasValue) && u.Email == command.Email, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Email), cancellationToken);
        await this.ValidateExistParamAsync<User>(u => (userId.HasValue && u.Id != userId.Value || !userId.HasValue) && u.PhoneNumber == command.Phone, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Phone), cancellationToken);
        await this.ValidateExistParamAsync<User>(u => (userId.HasValue && u.Id != userId.Value || !userId.HasValue) && u.Login == command.Login, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Login), cancellationToken);
    }
}