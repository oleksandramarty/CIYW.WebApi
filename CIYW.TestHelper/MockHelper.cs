using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
using CIYW.Mediator.Auth.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

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
            Email = "myemail@mail.com",
            EmailConfirmed = true,
            PhoneNumber = "12124567890",
            PhoneNumberConfirmed = true,
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
    
    public static CreateUserCommand CreateCreateUserCommand()
    {
        return new CreateUserCommand(
            "LastName",
            "FirstName",
            "Test",
            "Login",
            "email@mail.com",
            "12124567890",
            "email@mail.com",
            true,
            "Password123",
            "Password123",
            false);
    }
    
    public static void SetupData<T>(this Mock<DbSet<T>> dbSetMock, IEnumerable<T> data) where T : class
    {
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
    }

}