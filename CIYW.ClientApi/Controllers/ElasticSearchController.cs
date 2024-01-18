// using CIYW.ClientApi.Controllers.Base;
// using CIYW.Interfaces;
// using CIYW.MongoDB.Models.Image;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace CIYW.ClientApi.Controllers;
//
// [Route("api-ciyw/[controller]/v1")]
// [ApiController]
// [AllowAnonymous]
// public class ElasticSearchController: BaseController
// {
//     //private readonly IElasticClient _elasticClient;
//     private readonly IMongoDbRepository<ImageData> _mongoTest;
//
//     public ElasticSearchController(
//         //IElasticClient elasticClient, 
//         IMongoDbRepository<ImageData> mongoTest)
//     {
//        // _elasticClient = elasticClient;
//         _mongoTest = mongoTest;
//     }
//     
//     [HttpPost("MongoAdd/{FileTypeEnum}")]
//     public async Task<IActionResult> V1_GetInvoice22By222IdAsync(IFormFile imageData, CancellationToken cancellationToken)
//     {
//         byte[] imageByteArray = await ConvertIFormFileToByteArrayAsync(imageData);
//
//         await this._mongoTest.CreateAsync(new ImageData
//         {
//             Id = Guid.NewGuid(),
//             Data = imageByteArray
//         });
//         
//         return Ok();
//     }
//     
//     [HttpGet("MongoGetAll")]
//     public async Task<IActionResult> V1_GetInvoice22By234222IdAsync(CancellationToken cancellationToken)
//     {
//         var result = await this._mongoTest.GetAllAsync();
//         
//         return Ok(result.ToList());
//     }
//     
//     // [HttpGet("RemoveAll")]
//     // public async Task<IActionResult> V1_GetInvoiceBy222IdAsync(CancellationToken cancellationToken)
//     // {
//     //     //"Invoices": "invoices_index",
//     //     //"User": "users_index",
//     //     var deleteResponse1 = await _elasticClient.DeleteByQueryAsync<User>(d => d
//     //         .Index("users")
//     //         .Query(q => q.MatchAll())
//     //     );
//     //     
//     //     return Ok();
//     // }
//     //
//     // [HttpGet("RemoveAllIndexes")]
//     // public async Task<IActionResult> V1_GetInvoiceBy22IdAsync(CancellationToken cancellationToken)
//     // {
//     //     //"Invoices": "invoices_index",
//     //     //"User": "users_index",
//     //     var deleteResponse1 = await _elasticClient.Indices.DeleteAsync("users");
//     //     return Ok();
//     // }    
//     // [HttpGet("Test")]
//     // public async Task<IActionResult> V1_GetInvoiceBy22IdA412sync(CancellationToken cancellationToken)
//     // {
//     //     var response = await this._elasticClient.IndexDocumentAsync<UserSearchModel>(new UserSearchModel
//     //     {
//     //         Id = Guid.NewGuid(),
//     //         Created = DateTime.UtcNow,
//     //         Updated = DateTime.UtcNow,
//     //         UserBalance = null,
//     //         Email = "anime.kit@mail.com",
//     //         Login = "anime.kit",
//     //         FirstName = "First name",
//     //         LastName = "Last name",
//     //         PhoneNumber = "12345678904"
//     //     });
//     //     return Ok();
//     // }
//     //
//     // [HttpGet("GetAllUsers")]
//     // public async Task<IActionResult> V1_GetUsersyIdAsync(CancellationToken cancellationToken)
//     // {
//     //     var result = await _elasticClient.SearchAsync<UserSearchModel>(
//     //         s => s
//     //             .Query(q => q.MatchAll())
//     //             .Size(5000));
//     //     
//     //     return Ok(result.Documents.ToList());
//     // }
//     
//
// }