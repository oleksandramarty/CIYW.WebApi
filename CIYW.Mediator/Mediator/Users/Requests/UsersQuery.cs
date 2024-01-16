using CIYW.Domain.Models.User;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class UsersQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<User>>
{
    public UsersQuery(BaseFilterQuery query)
    {
        Ids = query.Ids;
        Paginator = query.Paginator;
        DateRange = query.DateRange;
        Sort = query.Sort;
    }
    
    public UsersQuery(BaseIdsListQuery? ids, Paginator? paginator, BaseDateRangeQuery? dateRange, BaseSortableQuery? sort)
    {
        Ids = ids;
        Paginator = paginator;
        DateRange = dateRange;
        Sort = sort;
    }
}