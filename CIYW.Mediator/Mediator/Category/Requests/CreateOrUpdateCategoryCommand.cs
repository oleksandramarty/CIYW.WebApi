using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Category.Requests;

public class CreateOrUpdateCategoryCommand: BaseNullableQuery, IRequest<Guid>
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Ico { get; set; }
    
    public bool IsActive { get; set; }
}