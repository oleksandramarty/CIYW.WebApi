using CIYW.Mediatr.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController: BaseController
{
    private readonly IMediator _mediatr;
    
    public UsersController(IMediator mediatr) : base(mediatr)
    {
        _mediatr = mediatr;
    }
    
    [HttpGet("Current")]
    [ProducesResponseType(typeof(CurrentUserResponse), 200)]
    public async Task<IActionResult> CurrentUserAsync(CancellationToken cancellationToken)
    {
        CurrentUserResponse result = await this._mediatr.Send(new CurrentUserQuery(), cancellationToken);
        return Ok(result);
    }
}