using System.Linq.Expressions;

namespace CIYW.Interfaces;

public interface IReadGenericRepository<T> where T : class
{
    Task<IList<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IList<T>> GetWithPaginationAsync(int page, int take, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(Guid id,  CancellationToken cancellationToken);
    Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
}
