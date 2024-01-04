namespace CIYW.Interfaces;

public interface ICurrentUserProvider
{
    Task<Guid> GetUserIdAsync(CancellationToken cancellationToken);
    Task IsUserInRoleAsync(string roleName, CancellationToken cancellationToken);
    Task<bool> HasUserInRoleAsync(string roleName, CancellationToken cancellationToken);
}