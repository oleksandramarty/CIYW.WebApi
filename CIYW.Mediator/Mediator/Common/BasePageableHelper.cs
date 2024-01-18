using System.Linq.Expressions;
using CIYW.Interfaces;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;

namespace CIYW.Mediator.Mediator.Common;

public class BasePageableHelper<T> where T: class
{
    private readonly IReadGenericRepository<T> repository;

    public BasePageableHelper(IReadGenericRepository<T> repository)
    {
        this.repository = repository;
    }

    protected async Task<ListWithIncludeHelper<TResponse>> GetPageableResponseAsync<TResponse>(Expression<Func<T, bool>> condition,
        BaseFilterQuery filter,
        CancellationToken cancellationToken,
        params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        return await this.repository.GetListWithIncludeAsync<TResponse>(condition, filter, cancellationToken, includeFuncs);
    }
}