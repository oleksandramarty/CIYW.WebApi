using CIYW.Const.Enums;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Categories;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Tariffs;
using CIYW.Domain.Models.Users;

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
            Id = InitConst.UserRoleId, Name = RoleEnum.USER.ToString(), NormalizedName = RoleProvider.User.ToUpper(),
            ConcurrencyStamp = InitConst.UserRoleId.ToString()
        };
    }
    
    public static Role GetAdminRole()
    {
        return new Role { Id = InitConst.AdminUserId, Name = RoleEnum.ADMIN.ToString(), NormalizedName = RoleProvider.Admin.ToUpper(), ConcurrencyStamp = InitConst.AdminUserId.ToString() };
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
            { Id = InitConst.FreeTariffId, Name = "Free", Description = "Base tariff", Created = DateTime.UtcNow, IsActive = true };
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
            Created = DateTime.UtcNow, IsActive = true
        };
    }
    
    public static Category GetSalaryCategory()
    {
        return new Category()
        {
            Id = InitConst.CategorySalaryId, Name = "Salary", Description = "Salary", Ico = "Salary",
            Created = DateTime.UtcNow, IsActive = true
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
        return new Currency() { Id = InitConst.CurrencyUsdId, Name = "US Dollar", Symbol = "$", IsoCode = "USD", IsActive = true};
    }

    public static List<string> GenerateRandomFullNames(List<string> names, List<string> surnames, List<string> patronymics, int count)
    {
        Random random = new Random();
        List<string> results = new List<string>();
        for (int i = 0; i < count; i++)
        {
            results.Add($"{names[random.Next(names.Count)]} {surnames[random.Next(surnames.Count)]} {patronymics[random.Next(patronymics.Count)]}");
        }

        return results;
    }
}