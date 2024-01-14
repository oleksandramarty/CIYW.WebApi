using CIYW.Models.Requests.Common;
using Nest;

namespace CIYW.Interfaces;

public interface IFilterProvider<T>
{
    IQueryable<T> Apply(IQueryable<T> query, BaseFilterQuery filter);
}