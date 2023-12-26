using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Kernel.Extensions;

public static class UserExtension
{
    public static List<IdentityUserLogin<Guid>> CreateUserLogins(this User user)
    {
        return new List<IdentityUserLogin<Guid>>
        {
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWLogin,
                ProviderKey = user.Login,
                ProviderDisplayName = user.Login
            },
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWEmail,
                ProviderKey = user.Email,
                ProviderDisplayName = user.Email
            },
            new IdentityUserLogin<Guid> {
                UserId = user.Id,
                LoginProvider = LoginProvider.CIYWPhone,
                ProviderKey = user.PhoneNumber,
                ProviderDisplayName = user.PhoneNumber
            }
        };
    }
}