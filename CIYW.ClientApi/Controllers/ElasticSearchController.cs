using CIYW.ClientApi.Controllers.Base;
using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.MongoDB.Models.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace CIYW.ClientApi.Controllers;

[Route("api-ciyw/[controller]/v1")]
[ApiController]
[AllowAnonymous]
public class ElasticSearchController: BaseController
{
    private readonly IElasticClient _elasticClient;

    public ElasticSearchController(IElasticClient elasticClient)
    {
       _elasticClient = elasticClient;
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
                .Size(5000));
        
        return Ok(result.Documents.ToList());
    }
}