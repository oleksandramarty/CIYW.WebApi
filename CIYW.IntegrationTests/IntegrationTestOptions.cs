using System.Security.Claims;
using CIYW.Const.Providers;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Models.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.IntegrationTests;

public class IntegrationTestOptions
{
    public Guid? ClaimUserId { get; set; }
    public bool WithHub { get; set; } = false;
    public bool WithHttpContextAccessorForTesting { get; set; } = true;
    
    public HubConnection MessagesHub { get; set; }
    
    public string Token { get; set; }

    public HttpContextAccessorForTesting GenerateClaims()
    {
        var claims = this.ClaimUserId.HasValue ? this.GetTestClaims(this.ClaimUserId.Value) : null;
        
        var identity = new ClaimsIdentity(claims, "IntegrationTestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContextAccessorForTesting = new HttpContextAccessorForTesting();
        httpContextAccessorForTesting.HttpContext = new DefaultHttpContext
        {
            User = this.ClaimUserId.HasValue ? claimsPrincipal : null
        };
        return httpContextAccessorForTesting;
    }
    
    public void UpdateClaims(IServiceScope? scope, Guid? userId = null)
    {
        var serviceProvider = scope.ServiceProvider;
        var updatedHttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        var claims = userId.HasValue ? this.GetTestClaims(userId.Value) : null;

        var identity = new ClaimsIdentity(claims, "IntegrationTestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        updatedHttpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = userId.HasValue ? claimsPrincipal : null
        };
    }
    
    public async Task SetUserToken(IntegrationTestBase testApplicationFactory)
    {
        if (!this.ClaimUserId.HasValue)
        {
            return;
        }
        
        using (var scope = testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == this.ClaimUserId.Value,
                CancellationToken.None);

            TokenResponse token = await mediator.Send(new AuthLoginQuery(
                    user.Login, 
                    user.Email, 
                    user.PhoneNumber, 
                    "zcbm13579", 
                    false),
                CancellationToken.None);
            
            this.Token = $"{token?.Scheme} {token?.Value}";
        }
    }
    
    public async Task StarHubs(IntegrationTestBase testApplicationFactory, string baseUrl)
    {
        if (!this.WithHubActivate())
        {
            return;
        }
        
        this.MessagesHub = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}hubs/messages", o =>
            {
                o.HttpMessageHandlerFactory = _ => testApplicationFactory.Server.CreateHandler();
                o.Headers.Add("Authorization", this.Token);
            })
            .Build();

        await this.MessagesHub.StartAsync();
        
        await this.MessagesHub.InvokeAsync("SendMessageToAllActiveUsersAsync", "Integration Tests SignalR connection started.");
    }
    
    public async Task StopHubs()
    {
        if (!this.WithHubActivate())
        {
            return;
        }
        
        this.MessagesHub.StopAsync();
    }
    
    private List<Claim> GetTestClaims(Guid userId)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim("provider", LoginProvider.CIYWLogin),
            new Claim("jti", InitConst.MockJtiId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };
        
        if (userId == InitConst.MockUserId)
        {
            claims.Add(new Claim("sub", "john.doe"));
            claims.Add(new Claim(ClaimTypes.Role, RoleProvider.User));
        }
        
        if (userId == InitConst.MockAuthUserId)
        {
            claims.Add(new Claim("sub", "anime.kit"));
            claims.Add(new Claim(ClaimTypes.Role, RoleProvider.User));
        }
        
        if (userId == InitConst.MockAdminUserId)
        {
            claims.Add(new Claim("admin", "anime.test"));
            claims.Add(new Claim(ClaimTypes.Role, RoleProvider.Admin));
        }

        return claims;
    }

    private bool WithHubActivate()
    {
        return this.ClaimUserId.HasValue && this.WithHub;
    }
}