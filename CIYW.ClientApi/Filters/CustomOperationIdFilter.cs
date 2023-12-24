using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CIYW.ClientApi.Filters;

public class CustomOperationIdFilter: IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var relativePath = context.ApiDescription.RelativePath;

        if (relativePath.StartsWith("api-ciyw/"))
        {
            relativePath = relativePath.Substring("api-ciyw/".Length);
        }

        operation.OperationId = $"{RemoveParametersFromRoute(relativePath).Replace("/", "_")}";
    }
    
    private string RemoveParametersFromRoute(string route)
    {
        return System.Text.RegularExpressions.Regex.Replace(route, "/{[^}]+}", "");
    }
}