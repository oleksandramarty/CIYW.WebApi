using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Models.Helpers;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;

public class MockReadGenericRepository<T> : IReadGenericRepository<T> where T : class
{
    private readonly List<T> _data;

    public MockReadGenericRepository(List<T> data)
    {
        _data = data ?? new List<T>();
    }

    public Task<IList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<T>>(_data.ToList());
    }

    public Task<IList<T>> GetWithPaginationAsync(int page, int take, CancellationToken cancellationToken)
    {
        var result = _data.Skip((page - 1) * take).Take(take).ToList();
        return Task.FromResult<IList<T>>(result);
    }

    public Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = _data.FirstOrDefault(item => GetIdFromEntity(item) == id);
        this.CheckEntityExistsAsync(result, cancellationToken);
        return Task.FromResult(result);
    }

    public Task<T> GetWithIncludeAsync(Func<T, bool> condition, CancellationToken cancellationToken, params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        var filteredData = _data.Where(condition).AsQueryable();
        foreach (var includeFunc in includeFuncs)
        {
            filteredData = includeFunc(filteredData);
        }
        var result = filteredData.FirstOrDefault();
        this.CheckEntityExistsAsync(result, cancellationToken);
        return Task.FromResult(result);
    }

    public Task<ListWithIncludeHelper<T>> GetListWithIncludeAsync(
        Func<T, bool> condition,
        BaseFilterQuery filter,
        CancellationToken cancellationToken,
        params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        var filteredData = _data.Where(condition).AsQueryable();
        foreach (var includeFunc in includeFuncs)
        {
            filteredData = includeFunc(filteredData);
        }

        var result = new ListWithIncludeHelper<T>
        {
            Entities = filteredData.ToList(),
            Total = filteredData.Count()
        };

        return Task.FromResult(result);
    }

    public Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        var result = _data.AsQueryable().FirstOrDefault(predicate.Compile());
        return Task.FromResult(result);
    }

    public Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        var result = _data.AsQueryable().Where(predicate.Compile()).ToList();
        return Task.FromResult<IList<T>>(result);
    }

    private Guid GetIdFromEntity(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        return (Guid)idProperty.GetValue(entity);
    }
    
    private async Task CheckEntityExistsAsync(T entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            throw new LoggerException($"{typeof(T).Name} {ErrorMessages.NotFound}", 404, null);
        }
    }
}
