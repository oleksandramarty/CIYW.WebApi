using CIYW.Domain.Models;
using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Category;

public class CategoryResponse: BaseWithDateEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Ico { get; set; }
    public bool IsActive { get; set; }
}