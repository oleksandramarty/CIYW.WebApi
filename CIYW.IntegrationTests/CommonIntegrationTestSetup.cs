using System.Linq.Expressions;
using System.Security.Claims;
using CIYW.Const.Enums;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Utils;
using CIYW.MongoDB.Models.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests;

public class CommonIntegrationTestSetup: IDisposable 
{
    protected HttpClient Client { get; set; }
    protected IntegrationTestBase testApplicationFactory;
    protected IntegrationTestOptions options { get;}
    
    public CommonIntegrationTestSetup()
    {
        this.options = new IntegrationTestOptions
        {
            ClaimUserId = InitConst.MockUserId,
            WithHub = false,
            WithHttpContextAccessorForTesting = true
        };
    }

    public CommonIntegrationTestSetup(
        Guid? claimUserId, 
        bool withHub = false)
    {
        this.options = new IntegrationTestOptions
        {
            ClaimUserId = claimUserId,
            WithHub = withHub,
            WithHttpContextAccessorForTesting = !withHub
        };
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
        this.testApplicationFactory = new IntegrationTestBase(this.options);
        this.Client = this.testApplicationFactory.CreateClient();

        if (this.options.WithHub)
        {
            this.options.SetUserToken(this.testApplicationFactory).Wait();
            this.options.StarHubs(this.testApplicationFactory, this.Client.BaseAddress.AbsoluteUri).Wait();            
        }
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        this.options.StopHubs().Wait();
        this.Dispose();
    }
    
    public void Dispose()
    {
        Client.Dispose();
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