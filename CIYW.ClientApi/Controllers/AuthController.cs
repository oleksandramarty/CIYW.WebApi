using System.Threading;
using System.Threading.Tasks;
using CIYW.ClientApi.Controllers.Base;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[AllowAnonymous]
public class AuthController: BaseController
{
    private readonly IMediator mediator;
    
    public AuthController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("SignIn")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_SignInAsync([FromBody]CreateUserCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("Login")]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    public async Task<IActionResult> V1_LoginAsync([FromBody]AuthLoginQuery query, CancellationToken cancellationToken)
    {
        TokenResponse result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("Logout")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<IActionResult> V1_LogoutAsync(CancellationToken cancellationToken)
    {
        await this.mediator.Send(new AuthLogoutQuery(), cancellationToken);
        return Ok(true);
    }

    [HttpPut("ChangePassword")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<IActionResult> V1_ChangePasswordAsync([FromBody]ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("Password/Forgot")]
    [ProducesResponseType(typeof(void), 200)]
    [AllowAnonymous]
    public async Task<IActionResult> V1_ForgotPasswordAsync([FromBody]ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        await this.mediator.Send(query, cancellationToken);
        return Ok();
    }

    [HttpPost("restore/{url}")]
    [AllowAnonymous]
    public async Task<IActionResult> V1_RestorePasswordAsync([FromBody] RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpGet("temporary")]
    [ProducesResponseType(typeof(bool), 200)]
    [Authorize]
    public async Task<IActionResult> V1_CheckTemporaryPasswordAsync(CancellationToken cancellationToken)
    {
        bool result = await this.mediator.Send(new CheckTemporaryPasswordQuery(), cancellationToken);
        return Ok(true);
    }
}