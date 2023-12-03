using CIYW.Domain.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Domain;

  public class DataContext : IdentityDbContext<User, Role, Guid>
  {
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Role>(entity => { entity.ToTable("Roles", "CIYW.User"); });
      modelBuilder.Entity<User>(entity => { entity.ToTable("Users", "CIYW.User"); });

      var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

      foreach (var fk in cascadeFKs)
        fk.DeleteBehavior = DeleteBehavior.Restrict;

      base.OnModelCreating(modelBuilder);
    }
  }