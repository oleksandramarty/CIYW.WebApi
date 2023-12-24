using CIYW.Models.Requests.Common;

namespace CIYW.Interfaces;

public interface IFilterProvider<T>
{
    IQueryable<T> Apply(IQueryable<T> query, BaseFilterQuery filter);
}