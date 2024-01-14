using System;
using System.Threading;
using System.Threading.Tasks;
using CIYW.Const.Enums;
using CIYW.Mediator.Mediator.Dictionary.Requests;
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
    public async Task<IActionResult> V1_GetAllAsync(CancellationToken cancellationToken)
    {
        DictionariesResponse response = await this.mediator.Send(new DictionaryQuery(), cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("{type}")]
    [ProducesResponseType(typeof(DictionaryResponse<Guid>), 200)]
    public async Task<IActionResult> V1_GetByTypeAsync([FromRoute] EntityTypeEnum type, CancellationToken cancellationToken)
    {
        DictionaryResponse<Guid> response = await this.mediator.Send(new DictionaryTypeQuery(type), cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("enum/{type}")]
    [ProducesResponseType(typeof(DictionaryResponse<string>), 200)]
    public async Task<IActionResult> V1_GetByEnumTypeAsync([FromRoute] EntityTypeEnum type, CancellationToken cancellationToken)
    {
        DictionaryResponse<string> response = await this.mediator.Send(new DictionaryEnumQuery(type), cancellationToken);
        return Ok(response);
    }
}