using System.Security.Claims;
using CIYW.Domain.Models.User;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Interfaces;

public interface IAuthRepository
{
    Task<string> GetAuthenticationTokenAsync(User user, string loginProvider, string tokenName);
    Task<User> FindUserByIdAsync(string userId);
    Task UpdateUserLoginsAsync(Guid userId, IList<IdentityUserLogin<Guid>> entity, CancellationToken cancellationToken);

    Task UpdateUserLoginsAsync(User user, CancellationToken cancellationToken);
    Task LogOutUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<User> FindUserByLoginAsync(string loginProvider, string providerKey);
    Task<IList<string>> GetRolesAsync(User user);
    Task<bool> CheckPasswordAsync(User user, string password);

    Task<IdentityResult> SetAuthenticationTokenAsync(User user, string loginProvider,
        string tokenName, string tokenValue);
}
