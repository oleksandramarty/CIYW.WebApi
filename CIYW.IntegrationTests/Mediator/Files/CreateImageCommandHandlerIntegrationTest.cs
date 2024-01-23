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
public class CreateImageCommandHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateCreateImageCommand()
    {   
        for (int i = 1; i <= 3; i++)
        {
            string fileName = $"Test{i}.png";
            yield return new TestCaseData(FileTypeEnum.USER_IMAGE, GetResourceTestFile(ResourceProvider.Images, fileName), InitConst.MockUserId);
            yield return new TestCaseData(FileTypeEnum.USER_IMAGE, GetResourceTestFile(ResourceProvider.Images, fileName), null);
        }
    }
    
    [Test, TestCaseSource(nameof(CreateCreateImageCommand))]
    public async Task Handle_ValidCreateImageCommand_ReturnsImageId(FileTypeEnum type, IFormFile file, Guid? entityId)
    {
        // Arrange
        CreateImageCommand query = new CreateImageCommand(type, file, entityId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new CreateImageCommandHandler(
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
            
            if (!entityId.HasValue)
            {
                entityId = InitConst.MockAdminUserId;
            }
            
            ImageData expected = await this.FindFileAsync<ImageData>(m => m.Id == result.Entity.Id && m.EntityId == entityId.Value && m.Type == type, CancellationToken.None);

            expected.Should().NotBeNull();
            expected.Name.Should().Be(file.Name);

            await this.DeleteFileAsync<ImageData>(m => m.Id == result.Entity.Id, CancellationToken.None);
        }
    }
}