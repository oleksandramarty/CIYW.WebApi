using System.Linq.Expressions;

namespace CIYW.Interfaces;

public interface IEntityValidator
{
    Task ValidateExistParamAsync<T>(Expression<Func<T, bool>> predicate, string customErrorMessage, CancellationToken cancellationToken) where T : class;
}