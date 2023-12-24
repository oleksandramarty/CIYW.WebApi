using CIYW.Const.Enum;
using CIYW.Domain.Models;
using CIYW.Domain.Models.Category;
using CIYW.Domain.Models.Currency;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;

namespace CIYW.Domain.Initialization;

    public static class DbInitializer
    {
      public static void Initialize(
          DataContext context,
          bool isProd)
      {
        AddEntitiesWithExisting<Role, Guid>(context, InitializationProvider.GetRoles(), c => c.Roles);
        AddEntitiesWithExisting<Tariff, Guid>(context, InitializationProvider.GetTariffs(), c => c.Tariffs);
        AddEntitiesWithExisting<Category, Guid>(context, InitializationProvider.GetCategories(), c => c.Categories);
        AddEntitiesWithExisting<Currency, Guid>(context, InitializationProvider.GetCurrencies(), c => c.Currencies);

        // AddTestInvoices(context);
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
    