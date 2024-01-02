using CIYW.Const.Enum;
using CIYW.Mediator.Mediatr.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize]
public class DictionariesController: BaseController
{
    private readonly IMediator mediator;
    
    public DictionariesController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpGet("")]
    [ProducesResponseType(typeof(DictionariesResponse), 200)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        DictionariesResponse response = await this.mediator.Send(new DictionaryQuery(), cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("{type}")]
    [ProducesResponseType(typeof(DictionaryResponse), 200)]
    public async Task<IActionResult> GetByTypeAsync([FromRoute] EntityTypeEnum type, CancellationToken cancellationToken)
    {
        DictionaryResponse response = await this.mediator.Send(new DictionaryTypeQuery(type), cancellationToken);
        return Ok(response);
    }
}