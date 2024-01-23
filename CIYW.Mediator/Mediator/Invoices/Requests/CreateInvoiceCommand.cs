using CIYW.Const.Enums;
using CIYW.Domain.Models.Invoices;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Models.Responses.Invoices;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Requests;

public class CreateInvoiceCommand: IRequest<MappedHelperResponse<InvoiceResponse, Invoice>>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public InvoiceTypeEnum Type { get; set; }
    public CreateOrUpdateNoteCommand? Note { get; set; }
}