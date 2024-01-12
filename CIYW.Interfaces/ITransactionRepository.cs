using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;

namespace CIYW.Interfaces;

public interface ITransactionRepository
{
    Task AddInvoiceAsync(
        Guid userId,
        Invoice invoice,
        Note note,
        CancellationToken cancellationToken);

    Task UpdateInvoiceAsync(
        Guid userId,
        Invoice invoice,
        Invoice updatedInvoice,
        Note note,
        CancellationToken cancellationToken);

    Task DeleteInvoiceAsync(
        Guid userId,
        Invoice invoice,
        CancellationToken cancellationToken);
}