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
public class DeleteImageCommandHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateDeleteImageCommand()
    {   
        for (int i = 1; i <= 3; i++)
        {
            string fileName = $"Test{i}.png";
            yield return new TestCaseData(FileTypeEnum.USER_IMAGE, GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockAuthUserId);
            yield return new TestCaseData(FileTypeEnum.USER_IMAGE, GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockUserId);
            yield return new TestCaseData(FileTypeEnum.USER_IMAGE, GetResourceTestFile(ResourceProvider.Images, fileName), null);
        }
    }
    
    [Test, TestCaseSource(nameof(CreateDeleteImageCommand))]
    public async Task Handle_ValidCreateImageCommand_ReturnsImageId(FileTypeEnum type, IFormFile file, Guid? entityId)
    {
        // Arrange
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            ImageData image = await this.CreateFileAsync<ImageData>(type, entityId ?? InitConst.MockAdminUserId, file, CancellationToken.None);
            
            DeleteImageCommand query = new DeleteImageCommand(image.Id);
            
            var handler = new DeleteImageCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
                );

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            if (!entityId.HasValue)
            {
                entityId = InitConst.MockAdminUserId;
            }
            
            ImageData expected = await this.FindFileAsync<ImageData>(m => m.Type == type && m.EntityId == entityId.Value, CancellationToken.None);

            if (expected != null)
            {
                await this.DeleteFileAsync<ImageData>(m => m.Id == image.Id, CancellationToken.None);
            }

            expected.Should().BeNull();
        }
    }
}