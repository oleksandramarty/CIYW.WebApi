using CIYW.Const.Enums;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class UpdateInvoiceCommand : BaseNullableQuery, IRequest<InvoiceResponse>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public InvoiceTypeEnum Type { get; set; }
    public CreateOrUpdateNoteCommand Note { get; set; }
}