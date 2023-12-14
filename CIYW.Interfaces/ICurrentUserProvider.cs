namespace CIYW.Interfaces;

public interface ICurrentUserProvider
{
    Task<Guid> GetUserIdAsync(CancellationToken cancellationToken);
}