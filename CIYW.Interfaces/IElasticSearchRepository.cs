using System.Linq.Expressions;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;

namespace CIYW.Interfaces;

public interface IElasticSearchRepository
{
    Task AddEntityAsync<T>(T entity, Guid id, string routeValue, CancellationToken cancellationToken) where T : class;
    Task AddEntitiesAsync<T>(List<T> entities, string schemeName, CancellationToken cancellationToken) where T : class;
    Task AddOrUpdateEntityAsync<T>(Expression<Func<T, bool>> predicate, Guid id, T entity, CancellationToken cancellationToken) where T : class;
    Task UpdateEntityAsync<T>(Expression<Func<T, bool>> predicate, Guid id, T entity,
        CancellationToken cancellationToken) where T : class;
    void DeleteById<T>(Expression<Func<T, bool>> predicate, Guid id) where T : class;
    Task<T?> GetByIdAsync<T>(Expression<Func<T, bool>> predicate, Guid id, CancellationToken cancellationToken) where T: class;
}