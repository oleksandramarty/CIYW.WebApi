using System.IdentityModel.Tokens.Jwt;
using CIYW.Auth;
using CIYW.Domain;
using CIYW.Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CIYW.Domain.Models.Users;
using CIYW.Elasticsearch;
using CIYW.Interfaces;
using CIYW.Jobs.Jobs;
using CIYW.Kernel.Extensions;
using CIYW.Kernel.Extensions.ActionFilters;
using CIYW.Repositories;
using CYIW.Mapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTransient<IPasswordHasher<User>, CIYWPasswordHasher>();

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CIYWConnection")));
        
        builder.Services.AddHangfire(configuration => configuration
            .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("CIYWConnectionHangfire")));

        builder.Services.AddIdentity<User, Role>(config =>
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

        builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
        
        builder.Services.AddMvc(mvcOptions =>
            {
                mvcOptions.CacheProfiles.Add("OneHour",
                    new CacheProfile {
                        Duration = 3600,
                        Location = ResponseCacheLocation.Any
                    });
                mvcOptions.CacheProfiles.Add("FiveMinutes",
                    new CacheProfile {
                        Duration = 300,
                        Location = ResponseCacheLocation.Any
                    });
                mvcOptions.CacheProfiles.Add("Week",
                    new CacheProfile {
                        Duration = 604800,
                        Location = ResponseCacheLocation.Any
                    });
                mvcOptions.CacheProfiles.Add("Month",
                    new CacheProfile {
                        Duration = 2419200,
                        Location = ResponseCacheLocation.Any
                    });
                mvcOptions.Filters.Add(typeof(ValidateModelStateAttribute));
                mvcOptions.Filters.Add(new HttpResponseExceptionFilter());
                mvcOptions.AllowEmptyInputInBodyModelBinding = false;
            }).AddXmlSerializerFormatters()
            .AddXmlDataContractSerializerFormatters()
            .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddElasticsearch(builder.Configuration);
           
        builder.Services.AddScoped<IElasticSearchRepository, ElasticSearchRepository>();
        builder.Services.AddScoped<IJobService, JobService>();
        
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); 
    
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Host.ConfigureContainer<ContainerBuilder>(opts =>
        {
            opts.RegisterModule(new MediatorModule());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new JobAuthorizationFilter() }
            });
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
        
        app.UseHangfireServer(new BackgroundJobServerOptions()
        {
            WorkerCount = 5
        });

        app.UseRouting();
        
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        app.InitJobs(cancellationToken);
        
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        
        app.Run();
    }
}

