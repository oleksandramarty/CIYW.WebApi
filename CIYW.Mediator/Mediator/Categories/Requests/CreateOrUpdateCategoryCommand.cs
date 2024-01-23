using CIYW.Domain.Models.Categories;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Categories;
using MediatR;

namespace CIYW.Mediator.Mediator.Categories.Requests;

public class CreateOrUpdateCategoryCommand: BaseNullableQuery, IRequest<MappedHelperResponse<CategoryResponse, Category>>
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Ico { get; set; }
    
    public bool IsActive { get; set; }
}