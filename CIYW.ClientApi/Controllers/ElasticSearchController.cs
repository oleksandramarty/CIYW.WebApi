using CIYW.ClientApi.Controllers.Base;
using CIYW.Const.Enums;
using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.MongoDB.Models.Image;
using CIYW.SignalR;
using CIYW.SignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nest;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[AllowAnonymous]
public class ElasticSearchController: BaseController
{
    private readonly IElasticClient _elasticClient;
    private readonly IHubContext<MessageHub> messageHub;

    public ElasticSearchController(IElasticClient elasticClient, IHubContext <MessageHub> messageHub)
    {
       _elasticClient = elasticClient;
       this.messageHub = messageHub;
    }
    
    [HttpGet("RemoveAll")]
    public async Task<IActionResult> V1_GetInvoiceBy222IdAsync(CancellationToken cancellationToken)
    {
        //"Invoices": "invoices_index",
        //"User": "users_index",
        var deleteResponse1 = await _elasticClient.DeleteByQueryAsync<User>(d => d
            .Index("users")
            .Query(q => q.MatchAll())
        );
        
        return Ok();
    }
    
    [HttpGet("RemoveAllIndexes")]
    public async Task<IActionResult> V1_GetInvoiceBy22IdAsync(CancellationToken cancellationToken)
    {
        //"Invoices": "invoices_index",
        //"User": "users_index",
        var deleteResponse1 = await _elasticClient.Indices.DeleteAsync("users");
        return Ok();
    }   
    
    [HttpGet(Name = "GetAll")]
    public async Task<IActionResult> Get(string keyword)
    {
        var result = await _elasticClient.SearchAsync<UserSearchModel>(
            s => s.MatchAll().Size(5000));

        return Ok(result.Documents.ToList());
    }
    
    [HttpPost("AddItem")]
    public async Task<IActionResult> V1_GetInvoiceBy22IdA412sync(UserSearchModel item, CancellationToken cancellationToken)
    {
        var response = await this._elasticClient.IndexDocumentAsync<UserSearchModel>(item);
        return Ok();
    }
    
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> V1_GetUsersyIdAsync(CancellationToken cancellationToken)
    {
        var result = await _elasticClient.SearchAsync<UserSearchModel>(
            s => s
                .Query(q => q.MatchAll())
                .Sort(sort => sort.Descending(s => s.Created))
                .Size(5000));
        
        return Ok(result.Documents.ToList());
    }
    
    [HttpGet("TestMessage")]
    public async Task<IActionResult> V1_GetUsersy11IdAsync(CancellationToken cancellationToken)
    {
        MessageHubModel message = new MessageHubModel(SignalRMessageTypeEnum.MESSAGE_TO_ALL_ACTIVE_USERS, "Test Message");
        await this.messageHub.Clients.All.SendAsync(message.SignalRMessageType.GetDescription(), message, cancellationToken);
        
        return Ok();
    }
    [HttpGet("TestMessage12")]
    public async Task<IActionResult> V1_GetUsersy111IdAsync(CancellationToken cancellationToken)
    {
        MessageHubModel message = new MessageHubModel(SignalRMessageTypeEnum.MESSAGE_TO_ALL_ACTIVE_USERS, "Test Message");
        await this.messageHub.Clients.All.SendAsync(message.SignalRMessageType.GetDescription(), message, cancellationToken);
        
        return Ok();
    }
}