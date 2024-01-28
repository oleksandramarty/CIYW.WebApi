using CIYW.Const.Enums;
using CIYW.Domain.Models.Tariffs;

namespace CIYW.Domain.Models.Currencies;

public class CurrencyEntity: BaseEntity
{
    public Guid EntityId { get; set; }
    public EntityTypeEnum EntityType { get; set; }
    public decimal Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public bool IsDefault { get; set; }
    public Guid? TariffId { get; set; }
    public Tariff Tariff { get; set; }
}