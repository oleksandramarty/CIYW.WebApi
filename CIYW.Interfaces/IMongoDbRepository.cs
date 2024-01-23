using System.Linq.Expressions;
using CIYW.Const.Enums;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;

namespace CIYW.Interfaces;

public interface IMongoDbRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
    Task CreateAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(Guid id, T entity);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ListWithIncludeHelper<TResponse>> GetPageableListAsync<TResponse>(BaseFileFilterQuery filter, CancellationToken cancellationToken);
}