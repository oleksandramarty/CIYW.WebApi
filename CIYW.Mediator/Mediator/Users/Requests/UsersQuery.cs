using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class UsersQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<UserResponse>>
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
    
    
    public bool? IsBlocked { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    
    public BaseDateRangeQuery? CreatedRange { get; set; }
    public BaseDateRangeQuery? UpdatedRange { get; set; }
    public BaseIdsListQuery CurrencyIds { get; set; }
    public BaseIdsListQuery RoleIds { get; set; }
    public BaseIdsListQuery TariffIds { get; set; }
}