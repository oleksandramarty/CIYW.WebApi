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

        query = this.ApplyIdsList(query, filter.Ids);
        query = this.ApplyPagination(query, filter.Paginator);
        query = this.ApplyDateRangeFilter(query, filter.DateRange);

        return query;
    }
    
    private PropertyInfo GetPropertyInfo(Type entityType, string columnName)
    {
        return entityType.GetProperty(columnName);
    }
    
    private IQueryable<T> ApplyIdsList(IQueryable<T> query, BaseIdsListQuery filter)
    {
        if (filter == null || filter.Ids == null || !filter.Ids.Any())
        {
            return query;
        }

        PropertyInfo idProperty = GetPropertyInfo(typeof(T), "Id");

        if (idProperty != null)
        {
            query = query.Where(q => filter.Ids.Contains((Guid)idProperty.GetValue(q)));
        }

        return query;
    }
    
    private IQueryable<T> ApplyPagination(IQueryable<T> query, BasePageableQuery filter)
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
            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return query;
    }
    
    private IQueryable<T> ApplyDateRangeFilter(IQueryable<T> query, BaseDateRangeQuery filter)
    {
        if (filter == null)
        {
            return query;
        }
        
        if (filter.DateFrom.HasValue && filter.DateFromColumn.NotNullOrEmpty())
        {
            PropertyInfo dateFromProperty = GetPropertyInfo(typeof(T), filter.DateFromColumn);
            
            query = query.Where(q => (DateTime?)dateFromProperty.GetValue(q) >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue && filter.DateToColumn.NotNullOrEmpty())
        {
            PropertyInfo dateToProperty = GetPropertyInfo(typeof(T), filter.DateToColumn);
            
            query = query.Where(q => (DateTime?)dateToProperty.GetValue(q) <= filter.DateTo.Value);
        }

        return query;
    }
}