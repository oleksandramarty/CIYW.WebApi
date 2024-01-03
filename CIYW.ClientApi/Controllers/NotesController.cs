using CIYW.Mediator.Mediator.Note.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize]
public class NotesController: BaseController
{
    private readonly IMediator mediator;
    
    public NotesController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> CreateNoteAsync(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Guid response = await this.mediator.Send(command, cancellationToken);
        return Ok(response);
    }
    
    [HttpPut("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> UpdateNoteAsync(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> DeleteNoteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteNoteCommand(id), cancellationToken);
        return Ok();
    }
}