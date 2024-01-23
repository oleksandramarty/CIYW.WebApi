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
public class UserImageQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUserImageQuery()
    {   
        for (int i = 1; i <= 3; i++)
        {
            string fileName = $"Test{i}.png";
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName), FileTypeEnum.USER_IMAGE, InitConst.MockUserId);
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName), FileTypeEnum.USER_IMAGE, null);
        }
    }
    
    [Test, TestCaseSource(nameof(CreateUserImageQuery))]
    public async Task Handle_ValidCreateImageCommand_ReturnsImageId(IFormFile file, FileTypeEnum type, Guid? entityId)
    {
        // Arrange
        ImageData image = await this.CreateFileAsync<ImageData>(
            FileTypeEnum.USER_IMAGE, 
            entityId ?? InitConst.MockAdminUserId, 
            file, 
            CancellationToken.None);
        
        UserImageQuery query = new UserImageQuery(image.EntityId)
        {
            Type = type,
            UserId = entityId
        };

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new UserImageQueryHandler(
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
            result.MappedEntity.Name.Should().Be(file.Name);
            result.Entity.Type.Should().Be(type);
            result.MappedEntity.Type.Should().Be(type);
            result.Entity.EntityId.Should().Be(entityId ?? InitConst.MockAdminUserId);
            result.MappedEntity.EntityId.Should().Be(entityId ?? InitConst.MockAdminUserId);

            await this.DeleteFileAsync<ImageData>(m => m.Id == result.Entity.Id, CancellationToken.None);
        }
    }
}