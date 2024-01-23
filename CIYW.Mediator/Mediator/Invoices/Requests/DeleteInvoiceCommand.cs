using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Requests;

public class DeleteInvoiceCommand: BaseQuery, IRequest
{
    public DeleteInvoiceCommand(Guid id)
    {
        Id = id;
    }
}