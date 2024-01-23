using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Files.Handlers;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Files;

[TestFixture]
public class UsersImagesQueryHandlerIntegrationTest(): CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUsersImagesQuery()
    {
        IFormFile file1 = GetResourceTestFile(ResourceProvider.Images, "Test1.png");
        IFormFile file2 = GetResourceTestFile(ResourceProvider.Images, "Test2.png");
        IFormFile file3 = GetResourceTestFile(ResourceProvider.Images, "Test3.png");
        
        yield return new TestCaseData(FileTypeEnum.USER_IMAGE, file1, file2, file3, InitConst.MockUserId, InitConst.MockAdminUserId, InitConst.AdminUserId, 3);
        yield return new TestCaseData(FileTypeEnum.USER_IMAGE, file1, file2, null, InitConst.MockUserId, InitConst.MockAdminUserId, InitConst.AdminUserId, 2);
        yield return new TestCaseData(FileTypeEnum.USER_IMAGE, file1, null, null, InitConst.MockUserId, InitConst.MockAdminUserId, InitConst.AdminUserId, 1);
        yield return new TestCaseData(FileTypeEnum.USER_IMAGE, null, null, null, InitConst.MockUserId, InitConst.MockAdminUserId, InitConst.AdminUserId, 0);
    }
    
    [Test, TestCaseSource(nameof(CreateUsersImagesQuery))]
    public async Task Handle_ValidCreateImageCommand_ReturnsImageId(
        FileTypeEnum type,
        IFormFile file1,
        IFormFile file2,
        IFormFile file3,
        Guid userId1,
        Guid userId2,
        Guid userId3,
        int expectedCount)
    {
        // Arrange
        ImageData image1 = file1 != null
            ? await this.CreateFileAsync<ImageData>(type, userId1, file1, CancellationToken.None)
            : null;
        ImageData image2 = file2 != null
            ? await this.CreateFileAsync<ImageData>(type, userId2, file2, CancellationToken.None)
            : null;
        ImageData image3 = file3 != null
            ? await this.CreateFileAsync<ImageData>(type, userId3, file3, CancellationToken.None)
            : null;

        UsersImagesQuery query = new UsersImagesQuery
        {
            Ids = new BaseIdsListQuery
            {
                Ids = new List<Guid> { userId1, userId2, userId3 }
            },
            Type = type
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new UsersImagesQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            ListWithIncludeHelper<ImageDataResponse> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Entities.Any().Should().Be(expectedCount > 0 );
            result.Entities.Count().Should().Be(expectedCount);
            result.TotalCount.Should().Be(expectedCount);

            if (image1 != null)
            {
                await this.DeleteFileAsync<ImageData>(m => m.Id == image1.Id, CancellationToken.None);               
            }
            if (image2 != null)
            {
                await this.DeleteFileAsync<ImageData>(m => m.Id == image2.Id, CancellationToken.None);               
            }
            if (image3 != null)
            {
                await this.DeleteFileAsync<ImageData>(m => m.Id == image3.Id, CancellationToken.None);               
            }
        }
    }
}