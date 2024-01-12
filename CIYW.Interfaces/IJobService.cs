namespace CIYW.Interfaces;

public interface IJobService
{
    Task TestJobAsync(CancellationToken cancellationToken);
}