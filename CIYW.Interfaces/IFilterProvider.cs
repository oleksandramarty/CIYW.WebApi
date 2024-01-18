using CIYW.Models.Requests.Common;
using MongoDB.Driver;
using Nest;

namespace CIYW.Interfaces;

public interface IFilterProvider<T>
{
    IQueryable<T> Apply(IQueryable<T> query, BaseFilterQuery filter);
    IFindFluent<T,T>? Apply(IFindFluent<T,T>? query, BaseFilterQuery filter);
}