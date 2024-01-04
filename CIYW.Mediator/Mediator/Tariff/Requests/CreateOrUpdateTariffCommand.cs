using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Requests;

public class CreateOrUpdateTariffCommand: BaseNullableQuery, IRequest<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
}