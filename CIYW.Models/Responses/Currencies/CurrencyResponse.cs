using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Currencies;

public class CurrencyResponse: BaseEntityResponse
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}