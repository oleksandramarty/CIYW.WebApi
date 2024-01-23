using CIYW.Domain.Models.Invoices;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Requests;

public class GetInvoiceByIdQuery: BaseQuery, IRequest<MappedHelperResponse<InvoiceResponse, Invoice>>
{
    public GetInvoiceByIdQuery(Guid id)
    {
        Id = id;
    }   
}