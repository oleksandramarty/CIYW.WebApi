using System.Threading;
using System.Threading.Tasks;
using CIYW.ClientApi.Controllers.Base;
using CIYW.Const.Providers;
using CIYW.Mediator.Mediator.Balances.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers.Admin;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize(Roles = RoleProvider.Admin)]
public class AdminCategoriesController: BaseController
{
    private readonly IMediator mediator;
    
    public AdminCategoriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_CreateCategoryAsync(CancellationToken cancellationToken)
    {
        decimal response = await this.mediator.Send(new UserBalanceQuery(), cancellationToken);
        return Ok(response);
    }
}