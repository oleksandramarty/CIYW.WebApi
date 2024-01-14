namespace CIYW.Interfaces;

public interface IJobService
{
    Task MapUsersAsync(CancellationToken cancellationToken);
    Task MapInvoicesAsync(CancellationToken cancellationToken);
}