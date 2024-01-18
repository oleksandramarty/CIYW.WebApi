using CIYW.Kernel.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers.Base;

[Route("api-ciyw/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status417ExpectationFailed)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status500InternalServerError)]
public class BaseController: ControllerBase
{
    public BaseController()
    {
    }
}