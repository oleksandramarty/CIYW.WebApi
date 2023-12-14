using CIYW.Mediatr.Invoice.Requests;
using CIYW.Mediatr.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvoiceController: BaseController
{
    private readonly IMediator _mediatr;
    
    public InvoiceController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> CreateInvoiceAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        await this._mediatr.Send(command, cancellationToken);
        return Ok();
    }
}