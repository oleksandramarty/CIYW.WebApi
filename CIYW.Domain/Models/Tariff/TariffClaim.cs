using CIYW.Const.Enum;

namespace CIYW.Domain.Models.Tariff;

public class TariffClaim: BaseWithDateEntity
{
    public Guid TariffId { get; set; }
    public Tariff Tariff { get; set; }
    
    public TariffClaimEnum Claim { get; set; }
}