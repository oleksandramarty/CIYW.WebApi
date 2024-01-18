using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class UserInvoicesQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<InvoiceResponse>>
{
    public UserInvoicesQuery(BaseFilterQuery query)
    {
        Ids = query.Ids;
        Paginator = query.Paginator;
        DateRange = query.DateRange;
        Sort = query.Sort;
    }
    
    public UserInvoicesQuery(BaseIdsListQuery? ids, Paginator? paginator, BaseDateRangeQuery? dateRange, BaseSortableQuery? sort)
    {
        Ids = ids;
        Paginator = paginator;
        DateRange = dateRange;
        Sort = sort;
    }
}