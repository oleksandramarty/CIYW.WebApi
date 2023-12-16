
using CIYW.Mediatr.Balance.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api-ciyw/[controller]")]
[ApiController]
[Authorize]
public class BalanceController: BaseController
{
    private readonly IMediator mediator;
    
    public BalanceController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpGet("")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetUserBalanceAsync(CancellationToken cancellationToken)
    {
        decimal response = await this.mediator.Send(new UserBalanceQuery(), cancellationToken);
        return Ok(response);
    }
}