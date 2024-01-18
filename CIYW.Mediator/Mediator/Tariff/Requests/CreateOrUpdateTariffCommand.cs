using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Requests;

public class CreateOrUpdateTariffCommand: BaseNullableQuery, IRequest<MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
}