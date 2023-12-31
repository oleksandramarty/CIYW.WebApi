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
using Autofac.Extensions.DependencyInjection;
using CIYW.Auth;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.ClientApi.Filters;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Mediator;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions.ActionFilters;
using CIYW.Repositories;
using CYIW.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi.Models;

namespace CIYW.IntegrationTests;

public class IntegrationTestBase: WebApplicationFactory<Program>
{
    public IntegrationTestBase()
    {
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
            User = claimsPrincipal
        };
        
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder
                .UseEnvironment("IntegrationTests")
                .UseTestServer()
                .ConfigureTestServices(services =>
                {
                    services.AddLogging();

                    var httpContextAccessorForTesting = new HttpContextAccessorForTesting();
                    services.AddSingleton<IHttpContextAccessor>(httpContextAccessorForTesting);
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