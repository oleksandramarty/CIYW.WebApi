using CIYW.Const.Enum;
using CIYW.Mediatr.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api-ciyw/[controller]")]
[ApiController]
[Authorize]
public class DictionaryController: BaseController
{
    private readonly IMediator mediator;
    
    public DictionaryController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpGet("")]
    [ProducesResponseType(typeof(DictionaryResponse), 200)]
    public async Task<IActionResult> GetUserInvoicesAsync(CancellationToken cancellationToken)
    {
        DictionaryResponse response = await this.mediator.Send(new DictionaryQuery(), cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("{type}")]
    [ProducesResponseType(typeof(IList<DictionaryItemResponse>), 200)]
    public async Task<IActionResult> GetUserInvoicesAsync([FromRoute] EntityTypeEnum type, CancellationToken cancellationToken)
    {
        IList<DictionaryItemResponse> response = await this.mediator.Send(new DictionaryTypeQuery(type), cancellationToken);
        return Ok(response);
    }
}