using Microsoft.AspNetCore.Http;

namespace CIYW.IntegrationTests;

public class HttpContextAccessorForTesting : IHttpContextAccessor
{
    public HttpContext HttpContext { get; set; }
}