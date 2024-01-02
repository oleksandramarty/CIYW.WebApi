using CIYW.Const.Enum;
using CIYW.Mediator.Mediatr.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediatr.Invoice.Requests;

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