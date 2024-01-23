using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Files.Handlers;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Files;

[TestFixture]
public class UpdateImageCommandHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateCreateImageCommand()
    {   
        for (int i = 2; i <= 3; i++)
        {
            string fileName = $"Test{i}.png";
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockAdminUserId);
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockAuthUserId);
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockUserId);
        }
    }
    
    [Test, TestCaseSource(nameof(CreateCreateImageCommand))]
    public async Task Handle_ValidCreateImageCommand_ReturnsImageId(IFormFile file, Guid userId)
    {
        // Arrange
        ImageData oldImage = await this.CreateFileAsync<ImageData>(
            FileTypeEnum.USER_IMAGE, 
            InitConst.MockUserId, 
            GetResourceTestFile(ResourceProvider.Images, "Test1.png"), 
            CancellationToken.None);
        
        UpdateImageCommand query = new UpdateImageCommand(oldImage.Id, file);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new UpdateImageCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<ImageDataResponse, ImageData> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Entity.Should().NotBeNull();
            result.MappedEntity.Should().NotBeNull();
            result.Entity.Name.Should().Be(file.Name);
            result.Entity.Name.Should().NotBeNull(oldImage.Name);

            await this.DeleteFileAsync<ImageData>(m => m.Id == result.Entity.Id, CancellationToken.None);
        }
    }
}