using AutoMapper;
using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Notes;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Models.Responses.Invoices;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Handlers;

public class CreateInvoiceCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Invoice>>
{
    private readonly IMapper mapper;
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public CreateInvoiceCommandHandler(
        IMapper mapper, 
        ITransactionRepository transactionRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<MappedHelperResponse<InvoiceResponse, Invoice>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Note note = null;

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        Invoice invoice = this.mapper.Map<CreateInvoiceCommand, Invoice>(command);
        
        invoice.UserId = userId;
        if (command.Note != null)
        {
            note = this.mapper.Map<CreateOrUpdateNoteCommand, Note>(command.Note, opts => opts.Items["IsUpdate"] = false);
            note.Id = Guid.NewGuid();
            note.UserId = userId;
            note.InvoiceId = invoice.Id;
            invoice.NoteId = note.Id;
        }

        await this.transactionRepository.AddInvoiceAsync(userId, invoice, note, cancellationToken);

        return this.GetMappedHelper<InvoiceResponse, Invoice>(invoice);
    }
}