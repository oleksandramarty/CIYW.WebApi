using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Invoice.Requests;
using CIYW.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Invoice.Handlers;

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

        if (command.NoteCommand != null)
        {
            note = this.mapper.Map<CreateNoteCommand, Domain.Models.Note.Note>(command.NoteCommand);
        }
        
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        Domain.Models.Invoice.Invoice invoice = this.mapper.Map<CreateInvoiceCommand, Domain.Models.Invoice.Invoice>(command);
        invoice.UserId = userId;
        invoice.NoteId = note?.Id;

        await this.transactionRepository.AddInvoiceAsync(userId, invoice, note, cancellationToken);

        return invoice.Id;
    }
}