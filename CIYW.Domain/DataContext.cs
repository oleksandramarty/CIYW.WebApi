using CIYW.Domain.Models;
using CIYW.Domain.Models.Categories;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Notes;
using CIYW.Domain.Models.Tariffs;
using CIYW.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Domain;

  public class DataContext : IdentityDbContext<User, Role, Guid>
  {
    public DbSet<Tariff> Tariffs { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserCategory> UserCategories { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<UserBalance> UserBalances { get; set; }
    public DbSet<ActiveUser> ActiveUsers { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Role>(entity => { entity.ToTable("Roles", "CIYW.Users"); });
      modelBuilder.Entity<User>(entity => { entity.ToTable("Users", "CIYW.Users"); });
      modelBuilder.Entity<UserCategory>(entity => { entity.ToTable("UserCategories", "CIYW.Users"); });
      modelBuilder.Entity<ActiveUser>(entity => { entity.ToTable("ActiveUsers", "CIYW.Users"); });
      
      modelBuilder.Entity<Currency>(entity => { entity.ToTable("Currencies", "CIYW.Dictionary"); });
      
      modelBuilder.Entity<Note>(entity => { entity.ToTable("Notes", "CIYW.Note"); });
      
      modelBuilder.Entity<Invoice>(entity => { entity.ToTable("Invoices", "CIYW.Invoice"); });
      
      modelBuilder.Entity<UserBalance>(entity => { entity.ToTable("UserBalances", "CIYW.Balance"); });
      
      modelBuilder.Entity<Tariff>(entity => { entity.ToTable("Tariffs", "CIYW.Tariff"); });
      
      modelBuilder.Entity<Category>(entity => { entity.ToTable("Categories", "CIYW.Category"); });

      var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

      foreach (var fk in cascadeFKs)
        fk.DeleteBehavior = DeleteBehavior.Restrict;
      
      modelBuilder.Entity<UserCategory>()
        .HasKey(c => new {c.UserId, c.CategoryId});
      
      modelBuilder.Entity<Invoice>()
        .HasOne(p => p.Note)
        .WithOne(a => a.Invoice)
        .HasForeignKey<Note>(a => a.InvoiceId);
      
      modelBuilder.Entity<User>()
        .HasOne(p => p.UserBalance)
        .WithOne(a => a.User)
        .HasForeignKey<UserBalance>(a => a.UserId);

      modelBuilder.Entity<Invoice>()
        .Property(p => p.NoteId)
        .IsRequired(false);

      modelBuilder.Entity<Note>()
        .Property(a => a.InvoiceId)
        .IsRequired(false);

      base.OnModelCreating(modelBuilder);
    }
  }