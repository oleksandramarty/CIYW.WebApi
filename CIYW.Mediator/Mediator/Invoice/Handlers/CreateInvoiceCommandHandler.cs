using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class CreateInvoiceCommandHandler: IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public CreateInvoiceCommandHandler(
        IMapper mapper, 
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = null;

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        Domain.Models.Invoice.Invoice invoice = this.mapper.Map<CreateInvoiceCommand, Domain.Models.Invoice.Invoice>(command);
        
        invoice.UserId = userId;
        if (command.NoteCommand != null)
        {
            note = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command.NoteCommand, opts => opts.Items["IsUpdate"] = false);
            note.Id = Guid.NewGuid();
            note.UserId = userId;
            note.InvoiceId = invoice.Id;
            invoice.NoteId = note.Id;
        }

        await this.transactionRepository.AddInvoiceAsync(userId, invoice, note, cancellationToken);

        return invoice.Id;
    }
}