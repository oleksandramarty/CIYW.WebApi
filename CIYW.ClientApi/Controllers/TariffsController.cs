using CIYW.ClientApi.Controllers.Base;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Tariffs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[AllowAnonymous]
public class TariffsController: BaseController
{
    private readonly IMediator mediator;
    
    public TariffsController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ListWithIncludeHelper<TariffResponse>), 200)]
    public async Task<IActionResult> V1_GetTariffsAsync(TariffsQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<TariffResponse> response = await this.mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}