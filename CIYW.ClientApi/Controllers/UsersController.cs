using System.Threading;
using System.Threading.Tasks;
using CIYW.ClientApi.Controllers.Base;
using CIYW.Domain.Models.User;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Image;
using CIYW.Models.Responses.Users;
using CIYW.MongoDB.Models.Image;
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
        MappedHelperResponse<UserResponse, User> result = await this.mediator.Send(new CurrentUserQuery(), cancellationToken);
        return Ok(result.MappedEntity);
    }
    
    [HttpGet("Avatar")]
    [ProducesResponseType(typeof(ImageDataResponse), 200)]
    public async Task<IActionResult> V1_GetCurrentUserImageAsync(CancellationToken cancellationToken)
    {
        MappedHelperResponse<ImageDataResponse, ImageData> result = await this.mediator.Send(new CurrentUserImageQuery(), cancellationToken);
        return Ok(result.MappedEntity);
    }
}