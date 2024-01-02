using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediatr.Invoice.Requests;

public class DeleteInvoiceCommand: BaseQuery, IRequest
{
    public DeleteInvoiceCommand(Guid id)
    {
        Id = id;
    }
}