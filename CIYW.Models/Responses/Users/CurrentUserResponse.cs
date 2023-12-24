using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Tariff;

namespace CIYW.Models.Responses.Users;

public class CurrentUserResponse: BaseWithDateEntityResponse
{
    public string Login { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }

    public bool IsTemporaryPassword { get; set; }
    public bool Need2FAuthentication { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    
    public Guid RoleId { get; set; }
    public string Role { get; set; }
    
    public CurrencyResponse Currency { get; set; }    
    public TariffResponse Tariff { get; set; }
    
    public decimal BalanceAmount { get; set; }
}