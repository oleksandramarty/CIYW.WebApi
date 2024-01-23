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
public class CurrentUserImageQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateCurrentUserImageQuery()
    {   
        for (int i = 1; i <= 3; i++)
        {
            string fileName = $"Test{i}.png";
            yield return new TestCaseData(GetResourceTestFile(ResourceProvider.Images, fileName));
        }
        yield return new TestCaseData(null);
    }
    
    [Test, TestCaseSource(nameof(CreateCurrentUserImageQuery))]
    public async Task Handle_ValidCurrentUserImageQuery_ReturnsImageIfExists(IFormFile file)
    {
        // Arrange
        CurrentUserImageQuery query = new CurrentUserImageQuery();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            if (file != null)
            {
                await this.CreateFileAsync<ImageData>(FileTypeEnum.USER_IMAGE, InitConst.MockUserId, file, CancellationToken.None);
            }
            else
            {
                await this.DeleteFileAsync<ImageData>(
                    m => m.EntityId == InitConst.MockUserId && m.Type == FileTypeEnum.USER_IMAGE,
                    CancellationToken.None);
            }
            
            var handler = new CurrentUserImageQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<ImageDataResponse, ImageData> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            if (file != null)
            {
                result.Should().NotBeNull();
                result.Entity.Should().NotBeNull();
                result.MappedEntity.Should().NotBeNull();

                result.Entity.EntityId.Should().Be(InitConst.MockUserId);
                result.Entity.Type.Should().Be(FileTypeEnum.USER_IMAGE);
                result.Entity.Name.Should().Be(file.Name);
                
                await this.DeleteFileAsync<ImageData>(m => m.Id == result.Entity.Id, CancellationToken.None);
            }
            else
            {
                result.Should().BeNull();
            }
        }
    }
}