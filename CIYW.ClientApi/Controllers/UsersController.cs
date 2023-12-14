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
    private IConfiguration _configuration;
    private readonly IMediator _mediatr;
    
    public UsersController(
        IConfiguration configuration, 
        IMediator mediatr) : base(configuration, mediatr)
    {
        _configuration = configuration;
        _mediatr = mediatr;
    }
    
    [HttpGet("Current")]
    [ProducesResponseType(typeof(CurrentUserResponse), 200)]
    public async Task<IActionResult> CurrentUserAsync(CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        CurrentUserResponse result = await this._mediatr.Send(new CurrentUserQuery(userId), cancellationToken);
        return Ok(result);
    }
}