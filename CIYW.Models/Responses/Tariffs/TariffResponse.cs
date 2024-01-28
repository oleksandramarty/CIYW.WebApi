using CIYW.Domain.Models.Currencies;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currencies;

namespace CIYW.Models.Responses.Tariffs;

public class TariffResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IList<CurrencyEntityResponse> CurrencyEntities { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }
}