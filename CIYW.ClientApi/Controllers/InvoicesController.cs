using CIYW.Domain.Models.Invoice;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Responses.Invoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[Authorize]
public class InvoicesController: BaseController
{
    private readonly IMediator mediator;
    
    public InvoicesController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    
    [HttpPost("history")]
    [ProducesResponseType(typeof(BalanceInvoicePageableResponse), 200)]
    public async Task<IActionResult> V1_GetUserInvoicesAsync(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        BalanceInvoicePageableResponse response = await this.mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(BalanceInvoicePageableResponse), 200)]
    public async Task<IActionResult> V1_GetUserInvoicesAsync(CancellationToken cancellationToken)
    {
        BalanceInvoicePageableResponse response = await this.mediator.Send(new UserMonthlyInvoicesQuery(), cancellationToken);
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BalanceInvoiceResponse), 200)]
    public async Task<IActionResult> V1_GetInvoiceByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        MappedHelperResponse<BalanceInvoiceResponse, Domain.Models.Invoice.Invoice> response = await this.mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        return Ok(response.MappedEntity);
    }
    
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> V1_CreateInvoiceAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Invoice response = await this.mediator.Send(command, cancellationToken);
        return Ok(response.Id);
    }
    
    [HttpPut("")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_UpdateInvoiceAsync(UpdateInvoiceCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<IActionResult> V1_DeleteInvoiceAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteInvoiceCommand(id), cancellationToken);
        return Ok();
    }
}