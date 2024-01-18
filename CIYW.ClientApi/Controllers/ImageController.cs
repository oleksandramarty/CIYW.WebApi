using CIYW.ClientApi.Controllers.Base;
using CIYW.Const.Enums;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Image;
using CIYW.MongoDB.Models.Image;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
public class ImageController: BaseController
{
    private readonly IMediator mediator;
    
    public ImageController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("{type}")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> V1_CreateImageAsync([FromRoute]FileTypeEnum type, IFormFile file, CancellationToken cancellationToken)
    {
        Guid id = await this.mediator.Send(new CreateImageCommand(type, file), cancellationToken);
        return Ok(id);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_UpdateImageAsync([FromRoute]Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new UpdateImageCommand(id, file), cancellationToken);
        return Ok(id);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_DeleteImageAsync([FromRoute]Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteImageCommand(id), cancellationToken);
        return Ok(id);
    }
    
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ImageDataResponse), 200)]
    public async Task<IActionResult> V1_GetUserImageAsync([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        ImageDataResponse result = await this.mediator.Send(new UserImageQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("users")]
    [ProducesResponseType(typeof(ListWithIncludeHelper<ImageDataResponse>), 200)]
    public async Task<IActionResult> V1_GetUsersImagesAsync(UsersImagesQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<ImageDataResponse> result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}