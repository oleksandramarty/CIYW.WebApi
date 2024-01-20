using CIYW.Elasticsearch.Models.Currency;

namespace CIYW.Elasticsearch.Models.User;

public class UserBalanceSearchModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public CurrencySearchModel Currency { get; set; }
}