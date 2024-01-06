using System.ComponentModel;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.ClientApi.Middleware;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.GraphQL;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions.ActionFilters;
using CIYW.Kernel.Utils;
using CIYW.Mediator.Validators.Categories;
using CIYW.Mediator.Validators.Currencies;
using CIYW.Mediator.Validators.Notes;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Namotion.Reflection;
using ZymLabs.NSwag.FluentValidation.AspNetCore;
using Int32Converter = CIYW.Kernel.Utils.Int32Converter;

namespace CIYW.Kernel.Extensions;

    public static class WebAppExtension
    {
        public static async Task RemoveDatabaseAsync(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    await context.Database.EnsureDeletedAsync();
                }
            }
        }

        public static async Task UpdateDatabaseAsync(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }
        
        public static void InitDatabase(
            this IApplicationBuilder app, 
            bool isProd,
            bool isIntegrationTests)
        {
            using (var serviceScope = app.ApplicationServices
                       .GetRequiredService<IServiceScopeFactory>()
                       .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                    DbInitializer.Initialize(context, userManager, isProd, isIntegrationTests);
                }
            }
        }

        public static void ConfigureApplicationLocalization(this IApplicationBuilder app)
        {
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
                SupportedCultures = new[] { CultureInfo.InvariantCulture },
                SupportedUICultures = new[] { CultureInfo.InvariantCulture }
            };

            // Remove any existing culture providers to avoid conflicts
            options.RequestCultureProviders.Clear();

            // Add your preferred culture provider (e.g., AcceptLanguageHeaderRequestCultureProvider or CookieRequestCultureProvider)
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            // UseRequestLocalization with the configured options
            app.UseRequestLocalization(options);
        }

        public static IApplicationBuilder UseRequestTimeHeader(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimeMiddleware>();
        }

        public static void AddJwtAuth(this WebApplicationBuilder builder)
        {
            builder.Services.AddTokenGenerator(options =>
            {
                options.JwtKey = builder.Configuration["JwtIssuer"];
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
        }

        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
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
            
            builder.AddSwagger();
        }
        
        public static void AddCorsSupport(this WebApplicationBuilder builder, string? origins)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(origins.Split(","))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        public static void AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddRouting(option => option.LowercaseUrls = true);
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder =
                        JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new Int32NullConverter());
                    options.JsonSerializerOptions.Converters.Add(new Int32Converter());
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation(fv =>
                {
                    fv.ValidatorOptions.DisplayNameResolver = (type, member, expression) =>
                        member != null ? string.Format("The field {0}", member.GetXmlDocsSummary().Trim()) : null;
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                });
        }

        public static void AddDependencyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            
            builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
            builder.Services.AddScoped(typeof(IFilterProvider<>), typeof(FilterProvider<>));
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IEntityValidator, EntityValidator>();
            builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped(typeof(IReadGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            builder.Services.AddScoped<ContextServiceLocator>();
        }

        public static void AddMvcSupport(this WebApplicationBuilder builder)
        {
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
                .AddXmlDataContractSerializerFormatters();
        }

        public static void DbMigrationWorker(this WebApplication app)
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            app.UpdateDatabaseAsync().Wait(cancellationToken);
            app.InitDatabase(app.Environment.IsProduction(), app.Environment.IsEnvironment("IntegrationTests"));
        }

        public static void AddFluentValidations(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrUpdateNoteCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrUpdateCategoryCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrUpdateTariffCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrUpdateCurrencyCommandValidator>();
        }

        public static void AddGraphQL(this WebApplicationBuilder builder)
        {
            //builder.Services.AddSingleton<ISchema, NotesSchema>(services => new NotesSchema(new SelfActivatingServiceProvider(services)));
            //builder.Services.AddSingleton<ISchema, CurrencySchema>(services => new CurrencySchema(new SelfActivatingServiceProvider(services)));
            //builder.Services.AddSingleton<ISchema, InvoiceSchema>(services => new InvoiceSchema(new SelfActivatingServiceProvider(services)));
            builder.Services.AddSingleton<ISchema, GraphQLSchema>(services => new GraphQLSchema(new SelfActivatingServiceProvider(services)));
            
            builder.Services.AddGraphQL(options =>
                options.ConfigureExecution((opt, next) =>
                {
                    opt.EnableMetrics = true;
                    opt.ThrowOnUnhandledException = true;
                    return next(opt);
                }).AddSystemTextJson()
            );
        }
    }
    