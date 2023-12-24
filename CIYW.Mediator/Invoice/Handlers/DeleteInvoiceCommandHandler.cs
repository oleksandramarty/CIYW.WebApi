using CIYW.Interfaces;
using CIYW.Mediator.Invoice.Requests;
using MediatR;

namespace CIYW.Mediator.Invoice.Handlers;

public class DeleteInvoiceCommandHandler: IRequestHandler<DeleteInvoiceCommand>
{
    private readonly ITransactionRepository transactionRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    
    public DeleteInvoiceCommandHandler(
        ITransactionRepository transactionRepository,
        ICurrentUserProvider currentUserProvider, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository)
    {
        this.transactionRepository = transactionRepository;
        this.currentUserProvider = currentUserProvider;
        this.invoiceRepository = invoiceRepository;
    }
    
    public async Task Handle(DeleteInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);

        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetByIdAsync(command.Id, cancellationToken);

        await this.transactionRepository.DeleteInvoiceAsync(userId, invoice, cancellationToken);
    }
}