using CIYW.Domain.Models.User;

namespace CIYW.Domain.Models.Category;

public class Category: BaseWithDateEntity
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Ico { get; set; }
    
    public bool IsActive { get; set; }
    
    public HashSet<UserCategory> UserCategories { get; set; }
    public HashSet<Invoice.Invoice> Invoices { get; set; }
}