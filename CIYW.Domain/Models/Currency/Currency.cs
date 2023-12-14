namespace CIYW.Domain.Models;

public class Currency: BaseEntity
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    
    public HashSet<User.User> Users { get; set; }
    public HashSet<Invoice.Invoice> Invoices { get; set; }
}