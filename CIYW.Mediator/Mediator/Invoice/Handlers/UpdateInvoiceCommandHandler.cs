using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class UpdateInvoiceCommandHandler: IRequestHandler<UpdateInvoiceCommand, Domain.Models.Invoice.Invoice>
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    private readonly IEntityValidator entityValidator;

    public UpdateInvoiceCommandHandler(
        IMapper mapper,
        IMediator mediator,
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
        this.invoiceRepository = invoiceRepository;
        this.entityValidator = entityValidator;
        this.mediator = mediator;
    }
    
    public async Task<Domain.Models.Invoice.Invoice> Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);

        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id.Value, cancellationToken);
        
        this.entityValidator.ValidateExist<Domain.Models.Invoice.Invoice, Guid?>(invoice, command.Id);
        
        Domain.Models.Invoice.Invoice updatedInvoice = this.mapper.Map<UpdateInvoiceCommand, Domain.Models.Invoice.Invoice>(command, invoice);
        
        Domain.Models.Note.Note note = null;
        
        if (command.Note != null)
        {
            note = this.mapper.Map<CreateOrUpdateNoteCommand, Domain.Models.Note.Note>(command.Note, opts => opts.Items["IsUpdate"] = true);
            note.Id = Guid.NewGuid();
            note.UserId = userId;
            note.InvoiceId = invoice.Id;
            invoice.NoteId = note.Id;
        }

        await this.transactionRepository.UpdateInvoiceAsync(userId, invoice, updatedInvoice, note, cancellationToken);
        
        

        return updatedInvoice;
    }
}