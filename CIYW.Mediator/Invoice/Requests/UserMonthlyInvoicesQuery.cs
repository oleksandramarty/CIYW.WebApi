using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Invoice.Requests;

public class UserMonthlyInvoicesQuery: BaseFilterQuery, IRequest<BalanceInvoicePageableResponse>
{
    
}