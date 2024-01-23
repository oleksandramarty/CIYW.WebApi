using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Tariffs;

public class TariffResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
}