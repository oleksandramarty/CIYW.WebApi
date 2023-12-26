using System.Linq.Expressions;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
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
}