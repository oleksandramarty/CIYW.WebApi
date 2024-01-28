using CIYW.ClientApi.Controllers.Base;
using CIYW.Resources.Forms.Definitions;
using CIYW.Resources.Forms.Enum;
using CIYW.Resources.Forms.Models;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers.Resources;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
public class DefinitionsController: BaseController
{
    private readonly IMediator mediator;
    
    public DefinitionsController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpGet("schemas/auth/{type}")]
    [ProducesResponseType(typeof(FormModel), 200)]
    [AllowAnonymous]
    public async Task<IActionResult> V1_GetAuthSchemaAsync([FromRoute] FormTypeEnum type, CancellationToken cancellationToken)
    {
        return Ok(this.GetSchema(type));
    } 
    
    [HttpGet("schemas/{type}")]
    [ProducesResponseType(typeof(FormModel), 200)]
    [Authorize]
    public async Task<IActionResult> V1_GetNonAuthSchemaAsync([FromRoute] FormTypeEnum type, CancellationToken cancellationToken)
    {
        return Ok(this.GetSchema(type));
    }

    private FormModel GetSchema(FormTypeEnum type)
    {
        return type.GenerateFormModel();
    }
}