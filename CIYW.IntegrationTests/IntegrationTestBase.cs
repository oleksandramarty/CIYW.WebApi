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
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CIYW.IntegrationTests;

public class IntegrationTestBase: WebApplicationFactory<Program>
{
    private Guid? claimUserId { get;}
    
    public IntegrationTestBase(Guid? claimUserId)
    {
        this.claimUserId = claimUserId;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var claims = this.claimUserId.HasValue
            ? new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, this.claimUserId.ToString()),
                new Claim(ClaimTypes.Role, this.GetRoleClaim(this.claimUserId)),
            }
            : null;

        var identity = new ClaimsIdentity(claims, "IntegrationTestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContextAccessorForTesting = new HttpContextAccessorForTesting();
        httpContextAccessorForTesting.HttpContext = new DefaultHttpContext
        {
            User = this.claimUserId.HasValue ? claimsPrincipal : null
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

    private string GetRoleClaim(Guid? userId)
    {
        if (!userId.HasValue)
        {
            return string.Empty;
        }
        
        return userId == InitConst.MockAdminUserId ? RoleProvider.Admin : RoleProvider.User;
    }
}