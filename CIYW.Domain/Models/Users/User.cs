using System.ComponentModel.DataAnnotations;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Notes;
using CIYW.Domain.Models.Notifications;
using CIYW.Domain.Models.Tariffs;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Domain.Models.Users;

public class User:IdentityUser<Guid>
{    
    [Required]
    [StringLength(50)]
    public string Login { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string? Patronymic { get; set; }

    public string Salt { get; set; }
    public bool IsTemporaryPassword { get; set; }
    public bool IsBlocked { get; set; } = false;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? LastForgot { get; set; }
    public DateTime? Mapped { get; set; }
    public DateTime? Restored { get; set; }
    
    public Guid TariffId { get; set; }
    public Tariff Tariff { get; set; }
    
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    
    public HashSet<Notification> Notifications { get; set; }
    public HashSet<UserCategory> UserCategories { get; set; }
    public HashSet<Invoice> Invoices { get; set; }
    public HashSet<Note> Notes { get; set; }
    public Guid UserBalanceId { get; set; }
    public UserBalance UserBalance { get; set; }
}