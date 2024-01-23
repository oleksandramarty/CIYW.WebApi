using System.Runtime.CompilerServices;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Users;
using CIYW.Models.Responses.Users;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Kernel.Extensions;

public static class UserExtension
{
    public static List<IdentityUserLogin<Guid>> CreateUserLogins(this User user)
    {
        return CreateUserLogins(user.Id, user.Login, user.PhoneNumber, user.Email);
    }
    public static List<IdentityUserLogin<Guid>> CreateUserLogins(this UserResponse user)
    {
        return CreateUserLogins(user.Id, user.Login, user.PhoneNumber, user.Email);
    }
    private static List<IdentityUserLogin<Guid>> CreateUserLogins(Guid userId, string login, string phone, string email)
    {
        return new List<IdentityUserLogin<Guid>>
        {
            new IdentityUserLogin<Guid> {
                UserId = userId,
                LoginProvider = LoginProvider.CIYWLogin,
                ProviderKey = login,
                ProviderDisplayName = login
            },
            new IdentityUserLogin<Guid> {
                UserId = userId,
                LoginProvider = LoginProvider.CIYWEmail,
                ProviderKey = email,
                ProviderDisplayName = email
            },
            new IdentityUserLogin<Guid> {
                UserId = userId,
                LoginProvider = LoginProvider.CIYWPhone,
                ProviderKey = phone,
                ProviderDisplayName = phone
            }
        };
    }
}