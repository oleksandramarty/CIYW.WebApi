using CIYW.Domain.Models.Tariffs;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Requests;

public class CreateOrUpdateTariffCommand: BaseNullableQuery, IRequest<MappedHelperResponse<TariffResponse, Tariff>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
}