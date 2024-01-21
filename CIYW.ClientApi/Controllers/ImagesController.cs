using CIYW.ClientApi.Controllers.Base;
using CIYW.Const.Enums;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Image;
using CIYW.MongoDB.Models.Image;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
public class ImagesController: BaseController
{
    private readonly IMediator mediator;
    
    public ImagesController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("{type}/{userId}")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> V1_CreateImageAsync([FromRoute]FileTypeEnum type, [FromRoute]Guid? userId, IFormFile file, CancellationToken cancellationToken)
    {
        Guid id = await this.mediator.Send(new CreateImageCommand(type, file, userId), cancellationToken);
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
}