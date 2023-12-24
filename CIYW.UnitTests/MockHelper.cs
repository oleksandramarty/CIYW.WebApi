using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
using Microsoft.AspNetCore.Identity;

namespace CIYW.UnitTests;

public static class MockHelper
{
    public static Tariff GetMockTariff()
    {
        return InitializationProvider.GetFreeTariff();
    }
    
    public static Currency GetMockCurrency()
    {
        return InitializationProvider.GetUSDCurrency();
    }
    
    public static Category GetMockCategory()
    {
        return InitializationProvider.GetOtherCategory();
    }
    
    public static Role GetMockRole()
    {
        return InitializationProvider.GetUserRole();
    }
    
    public static IList<IdentityUserRole<Guid>> GetMockRoles(Guid userId)
    {
        return new List<IdentityUserRole<Guid>>
        {
            new IdentityUserRole<Guid>
            {
                UserId = userId,
                RoleId = InitializationProvider.GetUserRole().Id
            }
        };
    }

    public static UserBalance GetMockBalance(Guid userId, Guid currencyId)
    {
        return new UserBalance
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CurrencyId = currencyId,
            Created = DateTime.UtcNow,
            Amount = 1000.0m,
        };
    }
    
    public static User GetMockUser()
    {
        Guid userId = Guid.NewGuid();
        
        Tariff tariff = GetMockTariff();
        Currency currency = GetMockCurrency();
        UserBalance userBalance = GetMockBalance(userId, currency.Id);
        
        return new User
        {
            Id = userId,
            Login = "john.doe",
            LastName = "Doe",
            FirstName = "John",
            Patronymic = "MiddleName",
            Salt = "some_salt",
            IsTemporaryPassword = false,
            Created = DateTime.UtcNow,
            Updated = null,
            LastForgot = null,
            TariffId = tariff.Id,
            Tariff = tariff,
            CurrencyId = currency.Id,
            Currency = currency,
            UserCategories = new HashSet<UserCategory>(),
            Invoices = new HashSet<Invoice>(),
            Notes = new HashSet<Note>(),
            UserBalanceId = userBalance.Id,
            UserBalance = userBalance,
        };
    }
}