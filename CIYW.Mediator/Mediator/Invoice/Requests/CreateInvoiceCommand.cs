using CIYW.Const.Enum;
using CIYW.Mediator.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class CreateInvoiceCommand: IRequest<Guid>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public InvoiceTypeEnum Type { get; set; }
    public CreateOrUpdateNoteCommand? NoteCommand { get; set; }
}