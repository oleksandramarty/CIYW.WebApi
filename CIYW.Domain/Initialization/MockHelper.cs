using CIYW.Const.Providers;
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

    public static User GetMockUser()
    {
        return CreateUser(
            InitConst.MockUserId,
            "john.doe",
            "Doe",
            "John",
            "Ivanovich",
            "myemail@mail.com",
            "12124567890",
            InitConst.FreeTariffId,
            InitConst.CurrencyUsdId
        );
    }
    
    private static User CreateUser(
        Guid userId,
        string login,
        string lastName,
        string firstName,
        string patronymic,
        string email,
        string phone,
        Guid tariffId,
        Guid currencyId
        )
      {
        UserBalance userBalance = InitializationProvider.GetUserBalance(userId, currencyId, 1000.0m);
        User user = new User
        {
          Id = userId,
          Login = login,
          LastName = lastName,
          FirstName = firstName,
          Patronymic = patronymic,
          Salt = "some_salt",
          Email = email,
          EmailConfirmed = true,
          PhoneNumber = phone,
          PhoneNumberConfirmed = true,
          IsTemporaryPassword = false,
          Created = DateTime.UtcNow,
          Updated = null,
          LastForgot = null,
          TariffId = tariffId,
          CurrencyId = currencyId,
          UserCategories = new HashSet<UserCategory>(),
          Invoices = new HashSet<Invoice>(),
          Notes = new HashSet<Note>(),
          UserBalanceId = userBalance.Id,
          UserBalance = userBalance,
        };

        return user;
      }
}