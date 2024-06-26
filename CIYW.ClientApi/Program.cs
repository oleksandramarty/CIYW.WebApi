using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using CIYW.Auth;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediatr;
using CIYW.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CIYW.Models.Mapping;

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

builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

//builder.Services.AddScoped<IHotelRoomRepository, HotelRoomRepository>();
builder.Services.AddRouting(option => option.LowercaseUrls = true);
builder.Services.AddControllers();
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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IReadGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); 

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new MediatrModule());
});

builder.Services.AddMvc();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Host.ConfigureContainer<ContainerBuilder>(
    builder => builder.RegisterModule(new MediatrModule()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.SerializeAsV2 = true);
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIYW.WebApi v1"));
}

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
app.UpdateDatabaseAsync().Wait(cancellationToken);
app.InitDatabase(app.Environment.IsProduction());

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();