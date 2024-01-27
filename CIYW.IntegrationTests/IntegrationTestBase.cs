using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using CIYW.Auth.Schemes;
using CIYW.ClientApi;
using CIYW.Const.Providers;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace CIYW.IntegrationTests;

public class IntegrationTestBase: WebApplicationFactory<Program>
{
    private IntegrationTestOptions options { get;}

    public IntegrationTestBase(IntegrationTestOptions options)
    {
        this.options = options;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder
                .UseEnvironment("IntegrationTests")
                .UseTestServer()
                .ConfigureTestServices(services =>
                {
                    if (options.WithHttpContextAccessorForTesting)
                    {
                        services.AddSingleton<IHttpContextAccessor>(options.GenerateClaims());                        
                    }
                })
                .ConfigureAppConfiguration(config =>
                {
                })
                .ConfigureTestContainer<ContainerBuilder>(opt =>
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