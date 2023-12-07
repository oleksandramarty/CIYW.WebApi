﻿using System.Globalization;
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
using CIYW.Interfaces;
using CIYW.Kernel.Extensions.ActionFilters;
using CIYW.Kernel.Utils;
using CIYW.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Namotion.Reflection;
using ZymLabs.NSwag.FluentValidation.AspNetCore;

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
        
        public static void InitDatabase(this IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices
                       .GetRequiredService<IServiceScopeFactory>()
                       .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    DbInitializer.Initialize(context, isProd);
                }
            }
        }

        public static void ConfigureApplicationLocalization(this IApplicationBuilder app)
        {
            //var english = "en-US";
            const string defCult = "en-US";
            var defaultRequestCulture = new RequestCulture(defCult, defCult);
            var supportedCultures = new List<CultureInfo>
            {
                new("en-US"),
                new("ru-RU")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultRequestCulture,
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            //RequestCultureProvider requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
            //requestProvider.Options.DefaultRequestCulture = englishRequestCulture;

            RequestCultureProvider requestProvider =
                options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First();
            options.RequestCultureProviders.Remove(requestProvider);

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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped(typeof(IReadGenericService<>), typeof(GenericService<>));
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
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
            app.InitDatabase(app.Environment.IsProduction());
        }
    }