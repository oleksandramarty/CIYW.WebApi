using System.Linq.Expressions;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class EntityValidator: IEntityValidator
{
    private readonly DataContext _dataContext;

    public EntityValidator(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task ValidateExistParamAsync<T>(Expression<Func<T, bool>> predicate, string customErrorMessage, CancellationToken cancellationToken) where T: class
    {
        T? entity = await _dataContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

        if (entity != null)
        {
            throw new LoggerException(customErrorMessage.NotNullOrEmpty() ? customErrorMessage : ErrorMessages.EntityAlreadyExists, 409);
        }
    }
    
    public void ValidateExist<T, TId>(T entity, TId? entityId) where T : class
    {
        if (entity == null)
        {
            throw new LoggerException(String.Format(ErrorMessages.EntityWithIdNotFound, typeof(T).Name, entityId), 404);
        }
    }

    public void ValidateRequest<TCommand, TResult>(TCommand command, Func<IValidator<TCommand>> validatorFactory) where TCommand : IRequest<TResult>
    {
        IValidator<TCommand> validator = validatorFactory.Invoke();
        ValidationResult validationResult = validator.Validate(command);
        
        if (validationResult.IsValid)
        {
           return;
        }
        
        throw new LoggerException(ErrorMessages.ValidationError, 409, null, validationResult.GetInvalidFieldInfo());
    }
    
    public void HasAccess<TEntity>(TEntity entity, Guid userId)
    {
        var propertyInfo = typeof(TEntity).GetProperty("UserId");

        if (propertyInfo != null)
        {
            var entityUserId = (Guid)propertyInfo.GetValue(entity);
            
            if (entityUserId != userId)
            {
                throw new LoggerException(ErrorMessages.Forbidden, 403, userId);
            }
            
            return;
        }

        throw new InvalidOperationException($"Type {typeof(TEntity).Name} does not have a UserId property.");
    }
}