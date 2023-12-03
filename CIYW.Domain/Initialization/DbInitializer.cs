using CIYW.Domain.Models.User;

namespace CIYW.Domain.Initialization;

    public static class DbInitializer
    {
      public static void Initialize(
          DataContext context,
          bool isProd)
      {
        AddRoles(context);
      }

      static void AddRoles(DataContext context)
      {
        IList<Role> roleAll = InitializationProvider.GetRoles();
        if (!context.Roles.Any())
        {
          context.Roles.AddRange(roleAll);
          context.SaveChanges();
        }
        else
        {
          var rolesExist = context.Roles.ToList();
          var existIds = rolesExist.Select(x => x.Id).ToList();
          var roleAllIds = roleAll.Select(x => x.Id).ToList();
          var rolesToAddIds = roleAllIds.Except(existIds).ToList();
          var rolesToAdd = roleAll.Where(_ => rolesToAddIds.Contains(_.Id)).ToList();
          if (rolesToAdd.Any())
          {
            context.Roles.AddRange(rolesToAdd);
            context.SaveChanges();
          }
        }
      }
    }