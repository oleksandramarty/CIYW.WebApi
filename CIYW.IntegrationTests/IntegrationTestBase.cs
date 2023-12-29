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

public class IntegrationTestBase: WebApplicationFactory<Microsoft.VisualStudio.TestPlatform.TestHost.Program>
{
    private readonly User mockUser;

    public IntegrationTestBase(User mockUser)
    {
        this.mockUser = mockUser;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, this.mockUser.Id.ToString())
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
                .UseEnvironment("Test")
                .UseTestServer()
                .ConfigureTestServices(services =>
                {
                    services.AddTransient<IPasswordHasher<User>, CIYWPasswordHasher>();
                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDatabaseForIntegrationTests")
                            .ConfigureWarnings(warnings =>
                                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                            .EnableSensitiveDataLogging();
                    });

                    services.AddTokenGenerator(options =>
                    {
                        options.JwtIssuer = "http://ciyw.com";
                        options.JwtKey = "3241ce7f-96ca-42ec-9943-7d2a2835d8f9";
                        options.JwtExpireDays = "1";
                    });

                    services.AddIdentity<User, Role>(config =>
                        {
                            config.User.RequireUniqueEmail = true;
                            config.Password.RequireDigit = true;
                            config.Password.RequiredLength = 6;
                            config.Password.RequireLowercase = false;
                            config.Password.RequireNonAlphanumeric = false;
                            config.Password.RequireUppercase = false;
                        })
                        .AddEntityFrameworkStores<DataContext>()
                        .AddDefaultTokenProviders();

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = JwtCIYWDefaults.AuthenticationScheme;
                            options.DefaultScheme = JwtCIYWDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtCIYWDefaults.AuthenticationScheme;
                        })
                        .AddScheme<JwtCIYWOptions, JwtCIYWHandler>(JwtCIYWDefaults.AuthenticationScheme,
                            options => { options.Realm = "Protect JwtCIYW"; });

                    services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

                    services.AddRouting(option => option.LowercaseUrls = true);
                    services.AddControllers();
                    services.AddOpenApiDocument();

                    services.AddEndpointsApiExplorer();

                    services.AddLogging();

                    var httpContextAccessorForTesting = new HttpContextAccessorForTesting();
                    services.AddSingleton<IHttpContextAccessor>(httpContextAccessorForTesting);

                    services.AddScoped<IDictionaryRepository, DictionaryRepository>();
                    services.AddScoped(typeof(IFilterProvider<>), typeof(FilterProvider<>));
                    services.AddScoped<ITransactionRepository, TransactionRepository>();
                    services.AddScoped<IEntityValidator, EntityValidator>();
                    services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
                    services.AddScoped<IAuthRepository, AuthRepository>();
                    services.AddScoped(typeof(IReadGenericRepository<>), typeof(GenericRepository<>));
                    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                    services.AddSingleton<IHttpContextAccessor>(httpContextAccessorForTesting);

                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
                })
                .ConfigureAppConfiguration(config =>
                {
                }).ConfigureTestContainer<ContainerBuilder>(opt =>
                {
                    opt.RegisterModule(new MediatrModule());
                }); //.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        });
        return base.CreateHost(builder);
    }
}