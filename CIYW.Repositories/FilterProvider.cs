using System.Reflection;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Models.Requests.Common;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace CIYW.Repositories;

public class FilterProvider<T>: IFilterProvider<T> where T : class
{
    public IQueryable<T> Apply(IQueryable<T> query, BaseFilterQuery filter)
    {
        if (filter == null)
        {
            return query;
        }

        query = this.ApplySort(query, filter.Sort);
        return this.ApplyPagination(query, filter.Paginator);
    }
    
    private IQueryable<T> ApplyPagination(IQueryable<T> query, Paginator filter)
    {
        if (filter == null)
        {
            return query;
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

    private IQueryable<T> ApplySort(IQueryable<T> query, BaseSortableQuery filter)
    {
        if (filter == null)
        {
            return query;
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Column))
        {
            if (string.Equals(filter.Direction, "asc", StringComparison.OrdinalIgnoreCase))
            {
                query = string.IsNullOrWhiteSpace(filter.ParentClass) ? 
                    query.OrderBy(x => EF.Property<object>(x, filter.Column)) :
                    query.OrderBy(x => EF.Property<object>(EF.Property<object>(x, filter.ParentClass), filter.Column));
            }
            else if (string.Equals(filter.Direction, "desc", StringComparison.OrdinalIgnoreCase))
            {
                query = string.IsNullOrWhiteSpace(filter.ParentClass) ? 
                    query.OrderByDescending(x => EF.Property<object>(x, filter.Column)) :
                    query.OrderByDescending(x => EF.Property<object>(EF.Property<object>(x, filter.ParentClass), filter.Column));
            }
        }

        return query;
    }
}