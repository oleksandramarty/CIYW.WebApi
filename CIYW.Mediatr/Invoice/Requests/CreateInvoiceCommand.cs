using CIYW.Mediatr.Note.Request;
using MediatR;

namespace CIYW.Mediatr.Invoice.Requests;

public class CreateInvoiceCommand: IRequest<Guid>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public CreateNoteCommand NoteCommand { get; set; }
}