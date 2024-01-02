using CIYW.Mediator.Mediatr.Users.Requests;
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
    [ProducesResponseType(typeof(CurrentUserResponse), 200)]
    public async Task<IActionResult> CurrentUserAsync(CancellationToken cancellationToken)
    {
        CurrentUserResponse result = await this.mediator.Send(new CurrentUserQuery(), cancellationToken);
        return Ok(result);
    }
}