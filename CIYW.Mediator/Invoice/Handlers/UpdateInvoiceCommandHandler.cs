using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Invoice.Requests;
using MediatR;

namespace CIYW.Mediator.Invoice.Handlers;

public class UpdateInvoiceCommandHandler: IRequestHandler<UpdateInvoiceCommand>
{
    private readonly IMapper mapper;
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    private readonly IEntityValidator entityValidator;

    public UpdateInvoiceCommandHandler(
        IMapper mapper, 
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
        this.invoiceRepository = invoiceRepository;
        this.entityValidator = entityValidator;
    }
    
    public async Task Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);

        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id, cancellationToken);
        
        this.entityValidator.ValidateExist<Domain.Models.Invoice.Invoice, Guid?>(invoice, command.Id);
        
        Domain.Models.Invoice.Invoice updatedInvoice = this.mapper.Map<UpdateInvoiceCommand, Domain.Models.Invoice.Invoice>(command, invoice);

        await this.transactionRepository.UpdateInvoiceAsync(userId, invoice, updatedInvoice, cancellationToken);
    }
}