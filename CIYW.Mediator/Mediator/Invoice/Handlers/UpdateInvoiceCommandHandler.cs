using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class UpdateInvoiceCommandHandler: UserEntityValidatorHelper, IRequestHandler<UpdateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>>
{
    private readonly IMapper mapper;
    private readonly ITransactionRepository transactionRepository;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;

    public UpdateInvoiceCommandHandler(
        IMapper mapper,
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.invoiceRepository = invoiceRepository;
    }
    
    public async Task<MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>> Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);

        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id.Value, cancellationToken);
        
        this.ValidateExist<Domain.Models.Invoice.Invoice, Guid?>(invoice, command.Id);
        
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

        return this.GetMappedHelper<InvoiceResponse, Domain.Models.Invoice.Invoice>(updatedInvoice);
    }
}