using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using CIYW.Domain.Initialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CIYW.IntegrationTests;

public class IntegrationTestBase: WebApplicationFactory<Program>
{
    private bool withClaims { get;}
    
    public IntegrationTestBase(bool withClaims = true)
    {
        this.withClaims = withClaims;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, InitConst.MockUserId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "IntegrationTestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContextAccessorForTesting = new HttpContextAccessorForTesting();
        httpContextAccessorForTesting.HttpContext = new DefaultHttpContext
        {
            User = this.withClaims ? claimsPrincipal : null
        };
        
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder
                .UseEnvironment("IntegrationTests")
                .UseTestServer()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IHttpContextAccessor>(httpContextAccessorForTesting);
                })
                .ConfigureAppConfiguration(config =>
                {
                }).ConfigureTestContainer<ContainerBuilder>(opt =>
                {
                });
        });
        return base.CreateHost(builder);
    }
}