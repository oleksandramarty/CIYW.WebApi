namespace CIYW.Domain.Models.Tariffs;

public class Tariff: BaseWithDateEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
    
    public HashSet<Users.User> Users { get; set; }
}