namespace CIYW.Domain.Models.User;

public class UserBalance: BaseWithDateEntity
{
    public decimal Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public Currency.Currency Currency { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}