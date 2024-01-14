using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CIYW.Domain.Models.User;
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

    public ElasticSearchController(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }
    
    [HttpGet("GetAllProducts/{keyword}")]
    public async Task<IActionResult> V1_GetInvoiceByIdAsync([FromRoute] string keyword, CancellationToken cancellationToken)
    {
        var result = await _elasticClient.SearchAsync<Product>(
            s => s.Query(
                q => q.QueryString(
                    d => d.Query('*' + keyword + '*')
                )).Size(5000));

        
        return Ok(result.Documents.ToList());
    }
    
    [HttpGet("GetAllUsers/{keyword}")]
    public async Task<IActionResult> V1_GetUsersyIdAsync([FromRoute] string keyword, CancellationToken cancellationToken)
    {
        var result = await _elasticClient.SearchAsync<User>(
            s => s.Query(
                q => q.QueryString(
                    d => d.Query('*' + keyword + '*')
                )).Size(5000));

        
        return Ok(result.Documents.ToList());
    }

    [HttpPost("AddOrUpdateProduct")]
    public async Task<IActionResult> Post(Product product, CancellationToken cancellationToken)
    {
        var result = await _elasticClient.SearchAsync<Product>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Id)
                    .Query(product.Id.ToString())
                )
            )
            .Size(1));
        
        if (result.Documents.Any())
        {
            _elasticClient.DeleteByQuery<Product>(p => p.Query(q1 => q1
                .Match(m => m
                    .Field(f => f.Id)
                    .Query(product.Id.ToString())
                )));
        }
        await _elasticClient.IndexDocumentAsync(product, cancellationToken);
        
        return Ok();
    }
    
    [HttpDelete("RemoveProductById/{productId}")]
    public async Task<IActionResult> RemoveProductById(string productId)
    {
        var response22 = _elasticClient.DeleteByQuery<Product>(p => p.Query(q1 => q1
            .Match(m => m
                .Field(f => f.Id)
                .Query(productId)
            )));

        return Ok();
    }
}