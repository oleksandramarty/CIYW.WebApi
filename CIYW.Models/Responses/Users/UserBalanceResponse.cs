using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currencies;

namespace CIYW.Models.Responses.Users;

public class UserBalanceResponse: BaseWithDateEntityResponse
{
    public decimal Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse Currency { get; set; }
    public Guid UserId { get; set; }
}