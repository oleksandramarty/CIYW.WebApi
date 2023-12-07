using CIYW.Kernel.Errors;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CIYW.Kernel.Extensions.ActionFilters;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (!filterContext.ModelState.IsValid)
        {
            filterContext.Result = new ErrorResult(filterContext.ModelState);
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext) { }
}