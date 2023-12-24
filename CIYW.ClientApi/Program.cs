using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using CIYW.Auth;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CIYW.ClientApi.Filters;
using CIYW.Kernel.Extensions.ActionFilters;
using CYIW.Mapper;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IPasswordHasher<User>, CIYWPasswordHasher>();
builder.Services.AddSignalR();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CIYWConnection")));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();
builder.Services.AddResponseCaching();

builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

//builder.Services.AddScoped<IHotelRoomRepository, HotelRoomRepository>();
builder.Services.AddRouting(option => option.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.AddDependencyInjection();

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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); 

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new MediatrModule());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.SerializeAsV2 = true);
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIYW.WebApi v1"));
}
else
{
    app.UseHsts();
}


var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

// if (app.Environment.IsDevelopment())
// {
//     app.RemoveDatabaseAsync().Wait(cancellationToken);
// }

app.UpdateDatabaseAsync().Wait(cancellationToken);
app.InitDatabase(app.Environment.IsProduction());

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();

app.UseRouting();

//app.ConfigureApplicationLocalization();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();