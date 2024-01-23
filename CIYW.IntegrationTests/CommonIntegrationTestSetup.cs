using System.Linq.Expressions;
using CIYW.Const.Enums;
using CIYW.Domain.Initialization;
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
    
    private Guid? claimUserId { get; }

    public CommonIntegrationTestSetup()
    {
        this.claimUserId = InitConst.MockUserId;
    }

    public CommonIntegrationTestSetup(Guid? claimUserId)
    {
        this.claimUserId = claimUserId;
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

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.testApplicationFactory = new IntegrationTestBase(this.claimUserId);
        this.Client = this.testApplicationFactory.CreateClient();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
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