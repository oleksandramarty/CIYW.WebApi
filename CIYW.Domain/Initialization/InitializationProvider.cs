using CIYW.Const.Enum;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;

namespace CIYW.Domain.Initialization;

public class InitializationProvider
{
    public static List<Role> GetRoles()
    {
        return new List<Role>
        {
            new Role { Id = InitConst.UserRoleId, Name = RoleEnum.User.ToString(), NormalizedName = RoleProvider.User.ToUpper(), ConcurrencyStamp = InitConst.UserRoleId.ToString() },
            new Role { Id = InitConst.AdminUserId, Name = RoleEnum.Admin.ToString(), NormalizedName = RoleProvider.Admin.ToUpper(), ConcurrencyStamp = InitConst.AdminUserId.ToString() }
        };
    }
    
    public static List<Tariff> GetTariffs()
    {
        return new List<Tariff>
        {
            new Tariff() { Id = InitConst.FreeTariffId, Name = "Free", Description = "Base tariff", Created = DateTime.UtcNow},
        };
    }

    public static List<TariffClaim> GetTariffClaims()
    {
        return new List<TariffClaim>
        {
            new TariffClaim
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Claim = TariffClaimEnum.Free,
                TariffId = InitConst.FreeTariffId
            }
        };
    }
}