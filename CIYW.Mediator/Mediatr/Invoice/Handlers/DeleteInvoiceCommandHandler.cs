using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Invoice.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Invoice.Handlers;

public class DeleteInvoiceCommandHandler: IRequestHandler<DeleteInvoiceCommand>
{
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    private readonly IEntityValidator entityValidator;
    
    public DeleteInvoiceCommandHandler(
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, 
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

        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Invoice.Invoice, Guid?>(invoice, command.Id);
        
        await this.transactionRepository.DeleteInvoiceAsync(userId, invoice, cancellationToken);
    }
}