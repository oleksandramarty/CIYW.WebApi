using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Users;

namespace CIYW.Domain.Models.Categories;

public class Category: BaseWithDateEntity
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Ico { get; set; }
    
    public bool IsActive { get; set; }
    
    public HashSet<UserCategory> UserCategories { get; set; }
    public HashSet<Invoice> Invoices { get; set; }
}