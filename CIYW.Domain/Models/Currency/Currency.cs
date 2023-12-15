using CIYW.Domain.Models.User;

namespace CIYW.Domain.Models.Currency;

public class Currency: BaseEntity
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    
    public HashSet<User.User> Users { get; set; }
    public HashSet<Invoice.Invoice> Invoices { get; set; }
    public HashSet<UserBalance> UserBalances { get; set; }
}