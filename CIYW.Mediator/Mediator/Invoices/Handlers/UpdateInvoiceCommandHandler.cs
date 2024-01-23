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

public class UpdateInvoiceCommandHandler: UserEntityValidatorHelper, IRequestHandler<UpdateInvoiceCommand, MappedHelperResponse<InvoiceResponse, Invoice>>
{
    private readonly IMapper mapper;
    private readonly ITransactionRepository transactionRepository;
    private readonly IReadGenericRepository<Invoice> invoiceRepository;

    public UpdateInvoiceCommandHandler(
        IMapper mapper,
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Invoice> invoiceRepository, IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.invoiceRepository = invoiceRepository;
    }
    
    public async Task<MappedHelperResponse<InvoiceResponse, Invoice>> Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);

        Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id.Value, cancellationToken);
        
        this.ValidateExist<Invoice, Guid?>(invoice, command.Id);
        
        Invoice updatedInvoice = this.mapper.Map<UpdateInvoiceCommand, Invoice>(command, invoice);
        
        Note note = null;
        
        if (command.Note != null)
        {
            note = this.mapper.Map<CreateOrUpdateNoteCommand, Note>(command.Note, opts => opts.Items["IsUpdate"] = true);
            note.Id = Guid.NewGuid();
            note.UserId = userId;
            note.InvoiceId = invoice.Id;
            invoice.NoteId = note.Id;
        }

        await this.transactionRepository.UpdateInvoiceAsync(userId, invoice, updatedInvoice, note, cancellationToken);

        return this.GetMappedHelper<InvoiceResponse, Invoice>(updatedInvoice);
    }
}