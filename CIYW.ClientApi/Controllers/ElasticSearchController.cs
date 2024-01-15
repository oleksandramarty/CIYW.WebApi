using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using Elasticsearch.Net;
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
    private readonly IJobService _jobService;

    public ElasticSearchController(IElasticClient elasticClient, IJobService jobService)
    {
        _elasticClient = elasticClient;
        _jobService = jobService;
    }
    
    [HttpGet("RemoveAll")]
    public async Task<IActionResult> V1_GetInvoiceBy222IdAsync(CancellationToken cancellationToken)
    {
        //"Invoices": "invoices_index",
        //"Users": "users_index",
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
        //"Users": "users_index",
        var deleteResponse1 = await _elasticClient.Indices.DeleteAsync("users");
        return Ok();
    }    
    [HttpGet("Test")]
    public async Task<IActionResult> V1_GetInvoiceBy22IdA412sync(CancellationToken cancellationToken)
    {
        var response = await this._elasticClient.IndexDocumentAsync<UserSearchModel>(new UserSearchModel
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            UserBalance = null,
            Email = "anime.kit@mail.com",
            Login = "anime.kit",
            FirstName = "First name",
            LastName = "Last name",
            PhoneNumber = "12345678904"
        });
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