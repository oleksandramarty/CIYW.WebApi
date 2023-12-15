using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Domain.Models.User;

public class User:IdentityUser<Guid>
{    
    [Required]
    [StringLength(50)]
    public string Login { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    [StringLength(50)]
    public string Patronymic { get; set; }

    public string Salt { get; set; }
    public bool IsTemporaryPassword { get; set; }
    public bool IsBlocked { get; set; } = false;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? LastForgot { get; set; }
    
    public Guid TariffId { get; set; }
    public Tariff.Tariff Tariff { get; set; }
    
    public Guid CurrencyId { get; set; }
    public Currency.Currency Currency { get; set; }
    
    public HashSet<UserCategory> UserCategories { get; set; }
    public HashSet<Invoice.Invoice> Invoices { get; set; }
    public HashSet<Note.Note> Notes { get; set; }
    public Guid UserBalanceId { get; set; }
    public UserBalance UserBalance { get; set; }
}