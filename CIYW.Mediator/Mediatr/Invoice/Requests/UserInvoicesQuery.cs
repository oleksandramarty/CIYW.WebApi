using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediatr.Invoice.Requests;

public class UserInvoicesQuery: BaseFilterQuery, IRequest<BalanceInvoicePageableResponse>
{
}