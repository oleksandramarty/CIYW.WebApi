using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Castle.Core.Configuration;
using CIYW.Auth;
using CIYW.Auth.Tokens;
using CIYW.Const.Enums;
using CIYW.Const.Providers;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Utils;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Models.Responses.Auth;
using CIYW.MongoDB.Models.Images;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace CIYW.IntegrationTests;

public class CommonIntegrationTestSetup: IDisposable 
{
    protected HttpClient Client { get; set; }
    protected IntegrationTestBase testApplicationFactory;
    protected HubConnection MessagesHub { get; set; }
    private Guid? claimUserId { get; }
    private bool IsNeedHub { get; }
    private string Token { get; set; }

    public CommonIntegrationTestSetup(bool isNeedHub = false)
    {
        this.claimUserId = InitConst.MockUserId;
        this.IsNeedHub = isNeedHub;
    }

    public CommonIntegrationTestSetup(Guid? claimUserId, bool isNeedHub = false)
    {
        this.claimUserId = claimUserId;
        this.IsNeedHub = isNeedHub;
    }
    
    protected static IFormFile GetResourceTestFile(string folder, string fileName)
    {
        string imagePath = Path.Combine("Resources", folder, fileName);
        byte[] jpegBytes = File.ReadAllBytes(imagePath);
        IFormFile file = new FormFile(new MemoryStream(jpegBytes), 0, jpegBytes.Length, fileName, fileName);
        return file;
    }
    
    protected async Task DeleteFileAsync<T>(Expression<Func<T,bool>> filter, CancellationToken cancellationToken) where T: class
    {
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            IMongoDbRepository<T> mongo = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<T>>();
            
            T current = (await mongo.FindAsync(filter, cancellationToken)).FirstOrDefault();
                
            if (current != null)
            {
                await mongo.DeleteAsync(ReflectionUtils.GetValue<T, Guid>(current, "Id"), cancellationToken);
            }
        }
    }
    
    protected async Task<T> FindFileAsync<T>(Expression<Func<T,bool>> filter, CancellationToken cancellationToken) where T: class
    {
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            IMongoDbRepository<T> mongo = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<T>>();
            
            return (await mongo.FindAsync(filter, cancellationToken)).FirstOrDefault();
        }
    }
    
    protected async Task<T> CreateFileAsync<T>(FileTypeEnum type, Guid entityId, IFormFile file, CancellationToken cancellationToken) 
        where T: class
    {
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            byte[] data = await this.ConvertIFormFileToByteArrayAsync(file, cancellationToken);
            if (typeof(T) == typeof(ImageData))
            {
                IMongoDbRepository<ImageData> mongo = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>();
                ImageData image = new ImageData(entityId, type, file.Name, data);
                await mongo.CreateAsync(image, cancellationToken);
                return (T)(object)image;
            }
            
            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }
    }

    protected async Task RemoveRestorePasswordDataAsync(CancellationToken cancellationToken)
    {
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            dbContext.Users.UpdateRange(
                dbContext.Users
                    .Where(u => u.Restored.HasValue)
                    .AsEnumerable()
                    .Select(x =>
                    {
                        x.Restored = null;
                        return x;
                    })
            );
            dbContext.RestorePasswords.RemoveRange(dbContext.RestorePasswords);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.testApplicationFactory = new IntegrationTestBase(this.claimUserId);
        this.Client = this.testApplicationFactory.CreateClient();
        this.SetUserToken().Wait();
        this.StarHubs().Wait();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        this.StopHubs().Wait();
        this.Dispose();
    }
    
    public void Dispose()
    {
        Client.Dispose();
    }

    public void SetClaims(IServiceScope? scope, Guid? userId = null)
    {
        if (!userId.HasValue && !this.claimUserId.HasValue)
        {
            return;
        }
        var serviceProvider = scope.ServiceProvider;
        var updatedHttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            
        var claims = DbInitializer.GetTestClaims(userId ?? this.claimUserId ?? Guid.Empty);

        var identity = new ClaimsIdentity(claims, "IntegrationTestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        updatedHttpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
    }

    private async Task SetUserToken()
    {
        if (!this.claimUserId.HasValue)
        {
            return;
        }
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == this.claimUserId.Value,
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

    private async Task StarHubs()
    {
        if (!this.claimUserId.HasValue || !this.IsNeedHub)
        {
            return;
        }
        
        this.MessagesHub = new HubConnectionBuilder()
            .WithUrl($"{this.Client.BaseAddress}hubs/messages", o =>
            {
                o.HttpMessageHandlerFactory = _ => testApplicationFactory.Server.CreateHandler();
                o.Headers.Add("Authorization", this.Token);
            })
            .Build();

        await this.MessagesHub.StartAsync();
        
        await this.MessagesHub.InvokeAsync("SendMessageToAllActiveUsersAsync", "Integration Tests SignalR connection started.");
    }
    
    private async Task StopHubs()
    {
        if (!this.claimUserId.HasValue || !this.IsNeedHub)
        {
            return;
        }
        
        this.MessagesHub.StopAsync();
    }
    
    private async Task<byte[]> ConvertIFormFileToByteArrayAsync(IFormFile avatarFile,
        CancellationToken cancellationToken)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await avatarFile.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }
    }
}