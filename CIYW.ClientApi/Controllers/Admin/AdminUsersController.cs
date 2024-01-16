using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers.Admin;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize(Roles = RoleProvider.Admin)]
public class AdminUsersController: BaseController
{
    private readonly IMediator mediator;
    
    public AdminUsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("filter")]
    [ProducesResponseType(typeof(ListWithIncludeHelper<User>), 200)]
    public async Task<IActionResult> V1_GetUsersAsync(UsersQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<User> response = await this.mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}