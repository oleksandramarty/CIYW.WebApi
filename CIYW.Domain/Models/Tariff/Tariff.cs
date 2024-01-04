namespace CIYW.Domain.Models.Tariff;

public class Tariff: BaseWithDateEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
    
    public HashSet<User.User> Users { get; set; }
}