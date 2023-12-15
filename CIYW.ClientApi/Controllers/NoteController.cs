using CIYW.Mediatr.Note.Request;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NoteController: BaseController
{
    private readonly IMediator mediator;
    
    public NoteController(IMediator mediator) : base(mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> CreateNoteAsync(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        Guid response = await this.mediator.Send(command, cancellationToken);
        return Ok(response);
    }
    
    [HttpPut("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> UpdateNoteAsync(UpdateNoteCommand command, CancellationToken cancellationToken)
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