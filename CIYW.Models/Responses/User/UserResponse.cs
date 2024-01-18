using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Tariff;

namespace CIYW.Models.Responses.Users;

public class UserResponse: BaseWithDateEntityResponse
{
    public string Login { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }
    public DateTime? LastForgot { get; set; }
    public bool IsTemporaryPassword { get; set; }
    public bool Need2FAuthentication { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    
    public Guid RoleId { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public Guid UserBalanceId { get; set; }
    public UserBalanceResponse UserBalance { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse Currency { get; set; }    
    public Guid TariffId { get; set; }
    public TariffResponse Tariff { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    
    public decimal BalanceAmount { get; set; }
}