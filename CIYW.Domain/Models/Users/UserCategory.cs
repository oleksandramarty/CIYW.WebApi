using CIYW.Domain.Models.Categories;

namespace CIYW.Domain.Models.Users;

public class UserCategory
{
    public Guid UserId { get; set; }
    public Users.User User { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}