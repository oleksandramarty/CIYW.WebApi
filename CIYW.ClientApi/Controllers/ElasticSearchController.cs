using CIYW.Kernel.Extensions;
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
    public async Task<IActionResult> V1_GetInvoiceByIdAsync([FromRoute] string keyword)
    {
        var result = await _elasticClient.SearchAsync<Product>(
            s => s.Query(
                q => q.QueryString(
                    d => d.Query('*' + keyword + '*')
                )).Size(5000));

        return Ok(result.Documents.ToList());
    }

    [HttpPost("AddProduct")]
    public async Task<IActionResult> Post(Product product)
    {
        // Index product dto
        await _elasticClient.IndexDocumentAsync(product);
        return Ok();
    }
}