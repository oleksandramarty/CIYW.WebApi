﻿using CIYW.Mediatr.Invoice.Requests;
using CIYW.Mediatr.Users.Requests;
using CIYW.Models.Responses.Invoice;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.Kernel.Extensions.Controllers;

[Route("api-ciyw/[controller]")]
[ApiController]
[Authorize]
public class InvoiceController: BaseController
{
    private readonly IMediator mediator;
    
    public InvoiceController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("history")]
    [ProducesResponseType(typeof(BalanceInvoicePageableResponse), 200)]
    public async Task<IActionResult> GetUserInvoicesAsync(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        BalanceInvoicePageableResponse response = await this.mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> CreateInvoiceAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid response = await this.mediator.Send(command, cancellationToken);
        return Ok(response);
    }
    
    [HttpPut("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> UpdateInvoiceAsync(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> DeleteInvoiceAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteInvoiceCommand(id), cancellationToken);
        return Ok();
    }
}