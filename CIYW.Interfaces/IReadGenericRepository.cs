using System.Linq.Expressions;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;

namespace CIYW.Interfaces;

public interface IReadGenericRepository<T> where T : class
{
    Task<IList<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IList<T>> GetWithPaginationAsync(int page, int take, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(Guid id,  CancellationToken cancellationToken);

    Task<T> GetWithIncludeAsync(Func<T, bool> condition, Func<IQueryable<T>, IQueryable<T>> includeFunc, CancellationToken cancellationToken);

    Task<ListWithIncludeHelper<T>> GetListWithIncludeAsync(
        Func<T, bool> condition,
        BaseFilterQuery filter,
        CancellationToken cancellationToken,
        params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs);
    Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
}
