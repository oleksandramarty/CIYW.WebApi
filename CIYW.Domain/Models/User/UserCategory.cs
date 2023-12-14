namespace CIYW.Domain.Models.User;

public class UserCategory
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category.Category Category { get; set; }
}