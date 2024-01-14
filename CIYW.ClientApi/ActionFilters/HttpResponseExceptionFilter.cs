using CIYW.Kernel.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CIYW.Kernel.Extensions.ActionFilters;

    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }
            switch (context.Exception)
            {
                case AuthenticationException authException:
                {
                  context.Result = new ObjectResult(authException.ToErrorMessage())
                  {
                    StatusCode = authException.statusCode
                  };
                  context.ExceptionHandled = true;
                  break;
                }

                case LoggerException exception:
                {
                    context.Result = new ObjectResult(exception.ToErrorMessage())
                    {
                        StatusCode = exception.statusCode
                    };
                    context.ExceptionHandled = true;
                    break;
                }
                case ValidationException validationException:
                {
                  context.Result = new ObjectResult(validationException.ToErrorMessage())
                  {
                    StatusCode = StatusCodes.Status400BadRequest
                  };
                  context.ExceptionHandled = true;
                  break;
                }
                default:
                {
                    context.Result = new ObjectResult(context.Exception.GetError())
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    context.ExceptionHandled = true;
                    break;
                }
            }
        }

        public int Order { get; set; } = int.MaxValue - 10;
    }