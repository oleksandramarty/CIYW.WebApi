using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Requests;

public class UserMonthlyInvoicesQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<InvoiceResponse>>
{
    
}