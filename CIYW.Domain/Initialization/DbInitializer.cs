using CIYW.Const.Enum;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
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
          
          AddUserIfNotExist(
            context,
            userManager,
            InitConst.MockAuthUserId,
            "anime.kit",
            "Anime",
            "Kit",
            "Kitovich",
            "animekit@mail.com",
            "22334433221",
            InitConst.FreeTariffId,
            InitConst.CurrencyUsdId,
            "zcbm13579",
            RoleProvider.User
          );
          
          AddUserIfNotExist(
            context,
            userManager,
            InitConst.MockAdminUserId,
            "admin.test",
            "Admin",
            "Test",
            "Adminovich",
            "admintest@mail.com",
            "44332255332",
            InitConst.FreeTariffId,
            InitConst.CurrencyUsdId,
            "zcbm13579",
            RoleProvider.Admin
          );
          
          AddTestInvoices(context, InitConst.MockUserId);
          AddTestInvoices(context, InitConst.MockAuthUserId);
          
          AddTestNotes(context, InitConst.MockUserId);
          AddTestNotes(context, InitConst.MockAuthUserId);
        }
        
        // AddTestInvoices(context, new Guid("f406bb8b-db38-47f8-a199-0191a56e93b1"));
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
          UserName = login,
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

        var result = Task.Run(() => userManager.CreateAsync(user, password)).Result;
        context.SaveChanges();
        result = Task.Run(() => userManager.AddToRoleAsync(user, role)).Result;
        context.SaveChanges();
        context.UserLogins.AddRange(logins);
        context.SaveChanges();
      }

      static void AddTestNotes(DataContext context, Guid userId)
      {
        User user = context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
          return;
        }
        
        var count = 0;
        
        IList<Note> notes = new List<Note>();
        
        for (var i = 0; i < 10; i++)
        {
          Note temp = new Note
          {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            Name = $"Name # {count++}",
            Body = $"Body # {count++}",
            UserId = user.Id
          };

          notes.Add(temp);
        }
        
        context.Notes.AddRange(notes);
        context.SaveChanges();
      }

      static void AddTestInvoices(DataContext context, Guid userId)
      {
        User user = context.Users.FirstOrDefault(u => u.Id == userId);
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
        
        for (var i = 0; i < 50; i++)
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
        
        for (var i = 0; i < 5; i++)
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
    