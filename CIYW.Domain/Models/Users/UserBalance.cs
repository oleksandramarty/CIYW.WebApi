using CIYW.Domain.Models.Currencies;

namespace CIYW.Domain.Models.Users;

public class UserBalance: BaseWithDateEntity
{
    public decimal Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public Guid UserId { get; set; }
    public Users.User User { get; set; }
}