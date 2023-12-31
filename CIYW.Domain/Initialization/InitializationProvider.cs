using CIYW.Const.Enum;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;

namespace CIYW.Domain.Initialization;

public class InitializationProvider
{
    public static List<Role> GetRoles()
    {
        return new List<Role>
        {
            GetUserRole(),
            GetAdminRole(),
        };
    }

    public static Role GetUserRole()
    {
        return new Role
        {
            Id = InitConst.UserRoleId, Name = RoleEnum.User.ToString(), NormalizedName = RoleProvider.User.ToUpper(),
            ConcurrencyStamp = InitConst.UserRoleId.ToString()
        };
    }
    
    public static Role GetAdminRole()
    {
        return new Role { Id = InitConst.AdminUserId, Name = RoleEnum.Admin.ToString(), NormalizedName = RoleProvider.Admin.ToUpper(), ConcurrencyStamp = InitConst.AdminUserId.ToString() };
    }
    
    public static List<Tariff> GetTariffs()
    {
        return new List<Tariff>
        {
            GetFreeTariff(),
        };
    }

    public static Tariff GetFreeTariff()
    {
        return new Tariff()
            { Id = InitConst.FreeTariffId, Name = "Free", Description = "Base tariff", Created = DateTime.UtcNow };
    }
    
    public static UserBalance GetUserBalance(Guid userId, Guid currencyId, decimal amount)
    {                                                                     
        return new UserBalance                                            
        {                                                                 
            Id = Guid.NewGuid(),                                          
            UserId = userId,                                              
            CurrencyId = currencyId,                                      
            Created = DateTime.UtcNow,                                    
            Amount = amount,                                             
        };                                                                
    }
    
    public static List<Category> GetCategories()
    {
        return new List<Category>
        {
            GetOtherCategory(),
            GetSalaryCategory(),
        };
    }

    public static Category GetOtherCategory()
    {
        return new Category()
        {
            Id = InitConst.CategoryOtherId, Name = "Other", Description = "Other", Ico = "Other",
            Created = DateTime.UtcNow
        };
    }
    
    public static Category GetSalaryCategory()
    {
        return new Category()
        {
            Id = InitConst.CategorySalaryId, Name = "Salary", Description = "Salary", Ico = "Salary",
            Created = DateTime.UtcNow
        };
    }
    
    public static List<Currency> GetCurrencies()
    {
        return new List<Currency>
        {
            GetUSDCurrency(),
        };
    }

    public static Currency GetUSDCurrency()
    {
        return new Currency() { Id = InitConst.CurrencyUsdId, Name = "US Dollar", Symbol = "$", IsoCode = "USD" };
    }
}