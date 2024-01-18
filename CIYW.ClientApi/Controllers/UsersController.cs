using System.Threading;
using System.Threading.Tasks;
using CIYW.ClientApi.Controllers.Base;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize]
public class UsersController: BaseController
{
    private readonly IMediator mediator;
    
    public UsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpGet("Current")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    public async Task<IActionResult> V1_CurrentUserAsync(CancellationToken cancellationToken)
    {
        UserResponse result = await this.mediator.Send(new CurrentUserQuery(), cancellationToken);
        return Ok(result);
    }
}