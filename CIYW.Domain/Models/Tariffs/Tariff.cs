using CIYW.Domain.Models.Currencies;

namespace CIYW.Domain.Models.Tariffs;

public class Tariff: BaseWithDateEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public HashSet<CurrencyEntity> CurrencyEntities { get; set; }
    public bool IsActive { get; set; }
    
    public HashSet<Users.User> Users { get; set; }
    public int Order { get; set; }
}