using System.Reflection;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Models.Requests.Common;

namespace CIYW.Repositories;

public class FilterProvider<T>: IFilterProvider<T> where T : class
{
    public IQueryable<T> Apply(IQueryable<T> query, BaseFilterQuery filter)
    {
        if (filter == null)
        {
            return query;
        }
        
        return this.ApplyPagination(query, filter.Paginator);
    }
    
    
    private IQueryable<T> ApplyPagination(IQueryable<T> query, BasePageableQuery filter)
    {
        if (filter == null)
        {
            return  query;
        }

        if (filter.PageNumber < 1)
        {
            filter.PageNumber = 1;
        }

        if (filter.PageSize < 1)
        {
            filter.PageSize = 5;
        }

        if (!filter.IsFull)
        {
            return query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return query;
    }
}