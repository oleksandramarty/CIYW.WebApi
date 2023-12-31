using CIYW.Const.Enum;
using CIYW.Const.Providers;
using CIYW.Domain.Models;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
using CIYW.UnitTests;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Domain.Initialization;

    public static class DbInitializer
    {
      public static void Initialize(
          DataContext context,
          UserManager<User> userManager,
          bool isProd,
          bool isIntegrationTests)
      {
        AddEntitiesWithExisting<Role, Guid>(context, InitializationProvider.GetRoles(), c => c.Roles);
        AddEntitiesWithExisting<Tariff, Guid>(context, InitializationProvider.GetTariffs(), c => c.Tariffs);
        AddEntitiesWithExisting<Category, Guid>(context, InitializationProvider.GetCategories(), c => c.Categories);
        AddEntitiesWithExisting<Currency, Guid>(context, InitializationProvider.GetCurrencies(), c => c.Currencies);

        // AddTestInvoices(context);

        if (isIntegrationTests)
        {
          AddUserIfNotExist(
            context,
            userManager,
            InitConst.MockUserId,
            "john.doe",
            "Doe",
            "John",
            "Ivanovich",
            "myemail@mail.com",
            "12124567890",
            InitConst.FreeTariffId,
            InitConst.CurrencyUsdId,
            "zcbm13579",
            RoleProvider.User
          );
        }
      }

      static void AddUserIfNotExist(
        DataContext context,
        UserManager<User> userManager,
        Guid userId,
        string login,
        string lastName,
        string firstName,
        string patronymic,
        string email,
        string phone,
        Guid tariffId,
        Guid currencyId,
        string password,
        string role
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
        
        List<IdentityUserLogin<Guid>> logins = new List<IdentityUserLogin<Guid>>
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

        if (context.Users.Any(u => u.Id == userId ||
                                   u.PhoneNumber == phone ||
                                   u.Email == email ||
                                   u.Login == login))
        {
          return;
        }
        
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        var res = userManager.CreateAsync(user, password);
        var temp2 = res.WaitAsync(cancellationToken);
        context.SaveChangesAsync(cancellationToken).Wait(cancellationToken);
        User temp = context.Users.FirstOrDefault(u => u.Id == userId);
        context.UserRoles.Add(new IdentityUserRole<Guid>
        {
          UserId = userId,
          RoleId = InitConst.UserRoleId
        });
        context.SaveChanges();
        context.UserLogins.AddRange(logins);
        context.SaveChanges();
      }

      static void AddTestInvoices(DataContext context)
      {
        User user = context.Users.FirstOrDefault();
        if (user == null)
        {
          return;
        }
        UserBalance userBalance = context.UserBalances.FirstOrDefault(_ => _.UserId == user.Id);
        if (userBalance == null)
        {
          return;
        }
        Random random = new Random();

        IList<Invoice> invoices = new List<Invoice>();
        DateTime today = DateTime.Now;

        var count = 0;
        
        for (var i = 0; i < 500; i++)
        {
          int randomDays = random.Next(0, 61);
          DateTime randomDate = today.AddDays(-randomDays);
          Invoice temp = new Invoice
          {
            Id = Guid.NewGuid(),
            Amount = (decimal)(random.NextDouble() * (1500 - 100) + 100),
            CurrencyId = InitConst.CurrencyUsdId,
            Type = InvoiceTypeEnum.Expense,
            CategoryId = InitConst.CategoryOtherId,
            Created = DateTime.UtcNow,
            Date = randomDate.ToUniversalTime(),
            Name = $"Invoice # {count++}",
            UserId = user.Id
          };
          
          userBalance.Amount = userBalance.Amount +
            (temp.Type == InvoiceTypeEnum.Income ? temp.Amount : (-1) * temp.Amount);

          invoices.Add(temp);
        }
        
        for (var i = 0; i < 50; i++)
        {
          int randomDays = random.Next(0, 61);
          DateTime randomDate = today.AddDays(-randomDays);
          Invoice temp = new Invoice
          {
            Id = Guid.NewGuid(),
            Amount = (decimal)(random.NextDouble() * (15000 - 100) + 100),
            CurrencyId = InitConst.CurrencyUsdId,
            Type = InvoiceTypeEnum.Income,
            CategoryId = InitConst.CategorySalaryId,
            Created = DateTime.UtcNow,
            Date = randomDate.ToUniversalTime(),
            Name = $"Invoice # {count++}",
            UserId = user.Id
          };
          
          userBalance.Amount = userBalance.Amount +
                               (temp.Type == InvoiceTypeEnum.Income ? temp.Amount : (-1) * temp.Amount);

          invoices.Add(temp);
        }
        
        userBalance.Updated = DateTime.UtcNow;
        
        context.Invoices.AddRange(invoices);
        context.UserBalances.Update(userBalance);

        context.SaveChanges();
      }
      
      static void AddEntitiesWithExisting<T, TId>(DataContext context, IList<T> entities, Func<DataContext, IQueryable<T>> getExistingEntities) where T : class
      {
        if (!getExistingEntities(context).Any())
        {
          context.Set<T>().AddRange(entities);
          context.SaveChanges();
        }
        else
        {
          var existingEntities = getExistingEntities(context).ToList();
          var existIds = existingEntities.Select(x => (TId)x.GetType().GetProperty("Id").GetValue(x)).ToList();
          var entityIds = entities.Select(x => (TId)x.GetType().GetProperty("Id").GetValue(x)).ToList();
          var entitiesToAddIds = entityIds.Except(existIds).ToList();
          var entitiesToAdd = entities.Where(_ => entitiesToAddIds.Contains((TId)_.GetType().GetProperty("Id").GetValue(_))).ToList();

          if (entitiesToAdd.Any())
          {
            context.Set<T>().AddRange(entitiesToAdd);
            context.SaveChanges();
          }
        }
      }
    }
    