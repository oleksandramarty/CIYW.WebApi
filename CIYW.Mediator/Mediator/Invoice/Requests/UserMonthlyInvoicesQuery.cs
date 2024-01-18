using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class UserMonthlyInvoicesQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<InvoiceResponse>>
{
    
}