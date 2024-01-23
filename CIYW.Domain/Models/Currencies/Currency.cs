using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Users;

namespace CIYW.Domain.Models.Currencies;

public class Currency: BaseEntity
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    
    public bool IsActive { get; set; }
    
    public HashSet<Users.User> Users { get; set; }
    public HashSet<Invoice> Invoices { get; set; }
    public HashSet<UserBalance> UserBalances { get; set; }
}