using CIYW.Models.Requests.Common;

namespace CIYW.Interfaces;

public interface IFilterProvider<T>
{
    IEnumerable<T> Apply(IEnumerable<T> query, BaseFilterQuery filter);
}