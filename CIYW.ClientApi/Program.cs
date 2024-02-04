using System.IdentityModel.Tokens.Jwt;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CIYW.Auth;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.ClientApi.Filters;
using CIYW.Domain;
using CIYW.Domain.Models.Users;
using CIYW.Elasticsearch;
using CIYW.Kernel.Extensions;
using CIYW.Kernel.Extensions.ActionFilters;
using CIYW.Mapper;
using CIYW.Mediator;
using CIYW.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace CIYW.ClientApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddTransient<IPasswordHasher<User>, CIYWPasswordHasher>();
        builder.Services.AddSignalR(e => {
            e.MaximumReceiveMessageSize = 102400000;
            e.EnableDetailedErrors = true;  
            e.KeepAliveInterval = TimeSpan.FromMinutes(1);  
        }).AddMessagePackProtocol();

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CIYWConnection")));
        
        builder.AddMongoDb();

        builder.Services.AddTokenGenerator(options =>
        {
            options.JwtIssuer = builder.Configuration["JwtIssuer"];
            options.JwtKey = builder.Configuration["JwtKey"];
            options.JwtExpireDays = builder.Configuration["JwtExpireDays"];
        });

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

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtCIYWDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtCIYWDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtCIYWDefaults.AuthenticationScheme;
            })
            .AddScheme<JwtCIYWOptions, JwtCIYWHandler>(JwtCIYWDefaults.AuthenticationScheme,
                options => { options.Realm = "Protect JwtCIYW"; });    

        var allowedSpecificOriginsPolicy = "_AllowedSpecificOriginsPolicy";

        string origin = builder.Configuration.GetValue<string>("Origin");
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(allowedSpecificOriginsPolicy, builder =>
            {
                builder.WithOrigins(origin.Split(","))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddResponseCompression();
        builder.Services.AddResponseCaching();

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

        builder.Services.AddRouting(option => option.LowercaseUrls = true);

        builder.AddFluentValidations();
        //builder.AddGraphQL();
        
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddOpenApiDocument();
        
        builder.Services.AddSwaggerDocument(config =>
        {
            config.DocumentName = "swagger";
        });
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CIYW.WebApi", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                              Enter 'JwtCIYW' [space] and then your token in the text input below.
                              \r\n\r\nExample: 'JwtCIYW 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "JwtCIYW"
            });
            
            c.OperationFilter<CustomOperationIdFilter>();

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
            // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // c.IncludeXmlComments(xmlPath);
        });
        
        builder.Services.AddSwaggerGenNewtonsoftSupport();
        
        builder.AddDependencyInjection();
        
        builder.Services.AddElasticsearch(builder.Configuration);
        
        builder.AddGraphQL();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); 

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Host.ConfigureContainer<ContainerBuilder>(opts =>
        {
            opts.RegisterModule(new MediatorModule());
        });

        var app = builder.Build();
        
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options => options.SerializeAsV2 = true);
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIYW.WebApi v1"));
            app.UseGraphQLPlayground("/graphql/playground");
        }
        else
        {
            app.UseHsts();
        }
        
        app.MapHub<MessageHub>("/hubs/messages");
        
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        if (app.Environment.IsEnvironment("IntegrationTests"))
        {
            app.RemoveDatabaseAsync().Wait(cancellationToken);
            app.RemoveFileDatabaseAsync(builder).Wait(cancellationToken);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        app.UpdateDatabaseAsync().Wait(cancellationToken);
        app.InitDatabase(builder, app.Environment.IsProduction(), app.Environment.IsEnvironment("IntegrationTests"));

        app.UseHttpsRedirection();
        app.UseResponseCompression();
        app.UseResponseCaching();

        app.UseRouting();

        app.ConfigureApplicationLocalization();

        app.UseCors(allowedSpecificOriginsPolicy);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        app.UseGraphQL();
        
        //   app.UseRouting();
        // app.UseEndpoints(endpoints =>
        //{
        //   endpoints.MapGraphQL();s
        //});

        app.Run();
    }
}