using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class CreateInvoiceCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>>
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

    public async Task<MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Domain.Models.Note.Note note = null;

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        Domain.Models.Invoice.Invoice invoice = this.mapper.Map<CreateInvoiceCommand, Domain.Models.Invoice.Invoice>(command);
        
        invoice.UserId = userId;
        if (command.Note != null)
        {
            note = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command.Note, opts => opts.Items["IsUpdate"] = false);
            note.Id = Guid.NewGuid();
            note.UserId = userId;
            note.InvoiceId = invoice.Id;
            invoice.NoteId = note.Id;
        }

        await this.transactionRepository.AddInvoiceAsync(userId, invoice, note, cancellationToken);

        return this.GetMappedHelper<InvoiceResponse, Domain.Models.Invoice.Invoice>(invoice);
    }
}