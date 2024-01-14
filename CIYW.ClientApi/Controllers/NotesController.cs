using System;
using System.Threading;
using System.Threading.Tasks;
using CIYW.Domain.Models.Note;
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
    public async Task<IActionResult> V1_CreateNoteAsync(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        Note response = await this.mediator.Send(command, cancellationToken);
        return Ok(response.Id);
    }
    
    [HttpPut("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_UpdateNoteAsync(CreateOrUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_DeleteNoteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteNoteCommand(id), cancellationToken);
        return Ok();
    }
}