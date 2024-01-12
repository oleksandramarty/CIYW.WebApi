using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class GetInvoiceByIdQuery: BaseQuery, IRequest<MappedHelperResponse<BalanceInvoiceResponse, Domain.Models.Invoice.Invoice>>
{
    public GetInvoiceByIdQuery(Guid id)
    {
        Id = id;
    }   
}