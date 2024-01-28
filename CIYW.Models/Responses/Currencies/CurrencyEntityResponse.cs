using CIYW.Const.Enums;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Tariffs;

namespace CIYW.Models.Responses.Currencies;

public class CurrencyEntityResponse: BaseEntityResponse
{
    public Guid EntityId { get; set; }
    public EntityTypeEnum EntityType { get; set; }
    public decimal Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse Currency { get; set; }
    public bool IsDefault { get; set; }
    public Guid? TariffId { get; set; }
    public TariffResponse Tariff { get; set; }
}