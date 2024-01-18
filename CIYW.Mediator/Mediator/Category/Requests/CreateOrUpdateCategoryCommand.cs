using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Category;
using MediatR;

namespace CIYW.Mediator.Mediator.Category.Requests;

public class CreateOrUpdateCategoryCommand: BaseNullableQuery, IRequest<MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>>
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Ico { get; set; }
    
    public bool IsActive { get; set; }
}