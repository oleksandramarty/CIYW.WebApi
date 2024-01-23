using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Invoices.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Handlers;

public class DeleteInvoiceCommandHandler: IRequestHandler<DeleteInvoiceCommand>
{
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IReadGenericRepository<Invoice> invoiceRepository;
    private readonly IEntityValidator entityValidator;
    
    public DeleteInvoiceCommandHandler(
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Invoice> invoiceRepository, 
        IEntityValidator entityValidator)
    {
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
        this.invoiceRepository = invoiceRepository;
        this.entityValidator = entityValidator;
    }
    
    public async Task Handle(DeleteInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);

        Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id, cancellationToken);

        this.entityValidator.ValidateExist<Invoice, Guid?>(invoice, command.Id);
        
        await this.transactionRepository.DeleteInvoiceAsync(userId, invoice, cancellationToken);
    }
}