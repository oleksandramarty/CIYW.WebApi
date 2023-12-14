using CIYW.Domain.Models.Tariff;
using CIYW.Domain.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Domain;

  public class DataContext : IdentityDbContext<User, Role, Guid>
  {
    public DbSet<Tariff> Tariffs { get; set; }
    public DbSet<TariffClaim> TariffClaims { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Role>(entity => { entity.ToTable("Roles", "CIYW.User"); });
      modelBuilder.Entity<User>(entity => { entity.ToTable("Users", "CIYW.User"); });
      
      
      modelBuilder.Entity<User>(entity => { entity.ToTable("Tariffs", "CIYW.Tariff"); });
      modelBuilder.Entity<User>(entity => { entity.ToTable("TariffClaims", "CIYW.Tariff"); });

      var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

      foreach (var fk in cascadeFKs)
        fk.DeleteBehavior = DeleteBehavior.Restrict;
      
      modelBuilder.Entity<TariffClaim>().HasOne(a => a.Tariff)
        .WithMany(b => b.Claims)
        .HasForeignKey(b => b.TariffId)
        .OnDelete(DeleteBehavior.Restrict);

      base.OnModelCreating(modelBuilder);
    }
  }