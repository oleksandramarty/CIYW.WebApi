﻿using CIYW.Domain.Models.Tariff;
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
    