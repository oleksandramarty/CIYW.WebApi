using CIYW.Const.Enums;
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

        if (isIntegrationTests || !isProd)
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

      public static void AddRandomUsers(
        DataContext context,
        UserManager<User> userManager,
        int count
      )
      {
            List<string> names = new List<string> { "Ethan", "Sophia", "Liam", "Isabella", "Noah", "Mia", "Mason", "Ava", "Aiden", "Amelia", "Lucas", "Harper", "Jackson", "Evelyn", "Logan", "Abigail", "Caleb", "Ella", "Jack", "Scarlett", "Benjamin", "Grace", "Oliver", "Lily", "Samuel", "Chloe", "Henry", "Aria", "Gabriel", "Sofia", "Carter", "Victoria", "Owen", "Madison", "Leo", "Penelope", "Wyatt", "Riley", "Isaac", "Zoe", "Lincoln", "Layla", "Anthony", "Nora", "Sebastian", "Aurora", "Julian", "Stella", "Elijah", "Hazel", "Dylan", "Lucy", "Caleb", "Hannah", "Nicholas", "Lillian", "Connor", "Grace", "Matthew", "Savannah", "David", "Eliana", "Joseph", "Addison", "Andrew", "Brooklyn", "Nathan", "Scarlett", "Christopher", "Natalie", "Joshua", "Lily", "Colton", "Samantha", "Zachary", "Leah", "Dominic", "Zoe", "Landon", "Aubrey", "Christian", "Emily", "Jonathan", "Sarah", "Aaron", "Claire", "Cameron", "Elizabeth", "Isaiah", "Layla", "Thomas", "Amelia", "Charles", "Audrey", "Eli", "Maya", "Hunter", "Anna", "Evan", "Paisley" };
            List<string> surnames = new List<string> { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales" };
            List<string> patronymics = new List<string> { "Johnson", "Robertson", "Williamson", "Anderson", "Davis", "Smithson", "Brown", "Miller", "Taylorson", "Jones", "Robinson", "Thomasson", "Harris", "Jackson", "Martinson", "Thompson", "Garcia", "Martinez", "Wilson", "Davisson", "Lewis", "Leeson", "Walkerson", "Hallson", "Allenson", "Young", "Hernandez", "Kingson", "Wrightson", "Lopezzon", "Scottson", "Greenson", "Adamsson", "Bakerson", "Gonzalez", "Nelson", "Carter", "Mitchellson", "Perez", "Robertsson", "Turnerson", "Phillipsson", "Campbell", "Parkerson", "Evanson", "Edwardsson", "Collinson", "Stewartsen", "Sanchezzon", "Morrison", "Rogersson", "Reedsen", "Cookson", "Morganson", "Bellson", "Murphy", "Baileysen", "Riverason", "Cooperson", "Richardsen", "Coxson", "Howardson", "Wardson", "Torresson", "Peterson", "Graysen", "Ramirezzon", "Jamesson", "Watsonson", "Brooksson", "Kellyson", "Sandersson", "Priceson", "Bennettson", "Woodson", "Bernesson", "Rossson", "Henderson", "Coleson", "Jenkinsson", "Perryson", "Powellson", "Longson", "Patterson", "Hughesson", "Floresson", "Washington", "Butlerson", "Simonsson", "Fosterson", "Gonzalez", "Mathewsson", "Josesson", "Tylerson", "Dylanson", "Pettersson", "Grayson", "Samson", "Parkssen", "Hanson" };
            
            List<string> randomFullNames = InitializationProvider.GenerateRandomFullNames(names, surnames, patronymics, count);

            List<UserBalance> userBalances = new List<UserBalance>();
            List<User> users = new List<User>();
            List<IdentityUserLogin<Guid>> logins = new List<IdentityUserLogin<Guid>>();

            foreach (var item in randomFullNames)
            {
              Guid userId = Guid.NewGuid();
              UserBalance userBalance = InitializationProvider.GetUserBalance(userId, InitConst.CurrencyUsdId, 0.0m);
              var temp = item.Split(" ");
              User user = new User
              {
                Id = userId,
                Login = $"{temp[1]}_{GenerateRandomString(6)}",
                UserName = $"{temp[1]}_{GenerateRandomString(6)}",
                LastName = temp[1],
                FirstName = temp[0],
                Patronymic = temp[2],
                Salt = "some_salt",
                Email = $"{temp[0]}.{temp[1]}_{GenerateRandomString(6)}@mail.com",
                EmailConfirmed = true,
                PhoneNumber = GenerateRandomString(11, true),
                PhoneNumberConfirmed = true,
                IsTemporaryPassword = false,
                Created = DateTime.UtcNow,
                Updated = null,
                LastForgot = null,
                TariffId = InitConst.FreeTariffId,
                CurrencyId = InitConst.CurrencyUsdId,
                UserCategories = new HashSet<UserCategory>(),
                Invoices = new HashSet<Invoice>(),
                Notes = new HashSet<Note>(),
                UserBalanceId = userBalance.Id,
                UserBalance = userBalance,
              };
              users.Add(user);
              logins.AddRange(new List<IdentityUserLogin<Guid>>
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
              });
            }

            foreach (var user in users)
            {
              var result = Task.Run(() => userManager.CreateAsync(user, "zcbm13579")).Result;
              context.SaveChanges();
              result = Task.Run(() => userManager.AddToRoleAsync(user, RoleProvider.User)).Result;
              context.SaveChanges();
            }
            context.UserLogins.AddRange(logins);
            context.SaveChanges();
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
            Type = InvoiceTypeEnum.EXPENSE,
            CategoryId = InitConst.CategoryOtherId,
            Created = DateTime.UtcNow,
            Date = randomDate.ToUniversalTime(),
            Name = $"Invoice # {count++}",
            UserId = user.Id
          };
          
          userBalance.Amount = userBalance.Amount +
            (temp.Type == InvoiceTypeEnum.INCOME ? temp.Amount : (-1) * temp.Amount);

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
            Type = InvoiceTypeEnum.INCOME,
            CategoryId = InitConst.CategorySalaryId,
            Created = DateTime.UtcNow,
            Date = randomDate.ToUniversalTime(),
            Name = $"Invoice # {count++}",
            UserId = user.Id
          };
          
          userBalance.Amount = userBalance.Amount +
                               (temp.Type == InvoiceTypeEnum.INCOME ? temp.Amount : (-1) * temp.Amount);

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
      
      private static string GenerateRandomString(int length, bool onlyDigits = false)
      {
        string chars = onlyDigits ? "123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();

        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
          result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
      }
    }
    