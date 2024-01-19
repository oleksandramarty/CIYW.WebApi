using CIYW.ClientApi.Controllers.Base;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Image;
using CIYW.Models.Responses.Users;
using CIYW.MongoDB.Models.Image;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers.Admin;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize(Roles = RoleProvider.Admin)]
public class AdminUsersController: BaseController
{
    private readonly IMediator mediator;
    
    public AdminUsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("filter")]
    [ProducesResponseType(typeof(ListWithIncludeHelper<UserResponse>), 200)]
    public async Task<IActionResult> V1_GetUsersAsync(UsersQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<UserResponse> response = await this.mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    
    [HttpPost("images/ids")]
    [ProducesResponseType(typeof(ListWithIncludeHelper<ImageDataResponse>), 200)]
    public async Task<IActionResult> V1_GetUsersImagesAsync(UsersImagesQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<ImageDataResponse> result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("images")]
    [ProducesResponseType(typeof(ImageDataResponse), 200)]
    public async Task<IActionResult> V1_GetUserImageAsync(UserImageQuery query, CancellationToken cancellationToken)
    {
        MappedHelperResponse<ImageDataResponse, ImageData> result = await this.mediator.Send(query, cancellationToken);
        return Ok(result.MappedEntity);
    }
}