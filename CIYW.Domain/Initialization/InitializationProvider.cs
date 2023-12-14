using CIYW.Const.Enum;
using CIYW.Const.Providers;
using CIYW.Domain.Models;
using CIYW.Domain.Models.Category;
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
    
    public static List<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category() { Id = InitConst.CategoryOtherId, Name = "Other", Description = "Other", Ico = "Other", Created = DateTime.UtcNow},
        };
    }
    
    public static List<Currency> GetCurrencies()
    {
        return new List<Currency>
        {
            new Currency() { Id = InitConst.CurrencyUsdId, Name = "US Dollar", Symbol = "$", IsoCode = "USD"},
        };
    }
}