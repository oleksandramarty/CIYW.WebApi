using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Categories;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Categories.Handlers;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Mediator.Mediator.Currencies.Handlers;
using CIYW.Mediator.Mediator.Currencies.Requests;
using CIYW.Mediator.Mediator.Files.Handlers;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Mediator.Mediator.Tariffs.Handlers;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Categories;
using CIYW.Models.Responses.Currencies;
using CIYW.Models.Responses.Images;
using CIYW.Models.Responses.Tariffs;
using CIYW.MongoDB.Models.Images;
using CIYW.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Common;

[TestFixture]
public class ForbiddenForUserHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockUserId)
{
    [Test]
    public async Task Handle_ForbiddenCreateCategoryCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateCategoryCommand command = new CreateOrUpdateCategoryCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CreateCategoryCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUpdateCategoryCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateCategoryCommand command = new CreateOrUpdateCategoryCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UpdateCategoryCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Category>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenCreateCurrencyCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateCurrencyCommand command = new CreateOrUpdateCurrencyCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CreateCurrencyCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currencies.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currencies.Currency>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUpdateCurrencyCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateCurrencyCommand command = new CreateOrUpdateCurrencyCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UpdateCurrencyCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currencies.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currencies.Currency>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenCreateTariffCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateTariffCommand command = new CreateOrUpdateTariffCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CreateTariffCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariffs.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariffs.Tariff>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUpdateTariffCommandHandler_ReturnsException()
    {
        // Arrange
        CreateOrUpdateTariffCommand command = new CreateOrUpdateTariffCommand();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new UpdateTariffCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariffs.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariffs.Tariff>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenCreateImageCommandForNonAdminWithDifferentUserId_ReturnsException()
    {
        // Arrange
        CreateImageCommand command = new CreateImageCommand(
            FileTypeEnum.USER_IMAGE, 
            GetResourceTestFile(ResourceProvider.Images, "Test1.png"), 
            InitConst.MockAuthUserId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CreateImageCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateImageCommand, MappedHelperResponse<ImageDataResponse, ImageData>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenDeleteImageCommandForNonAdminWithDifferentUserId_ReturnsException()
    {
        // Arrange
        ImageData image = await this.CreateFileAsync<ImageData>(
            FileTypeEnum.USER_IMAGE, 
            InitConst.MockAuthUserId, 
            GetResourceTestFile(ResourceProvider.Images, "Test1.png"),
            CancellationToken.None);
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new DeleteImageCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );
            
            // Act
            await TestUtilities.Handle_InvalidCommand<DeleteImageCommand, LoggerException>(
                handler, 
                new DeleteImageCommand(image.Id), 
                ErrorMessages.Forbidden,
                async () =>
                {
                    await this.DeleteFileAsync<ImageData>(m => m.Id == image.Id, CancellationToken.None);
                });
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUpdateImageCommandForNonAdminWithDifferentUserId_ReturnsException()
    {
        // Arrange
        IFormFile file = GetResourceTestFile(ResourceProvider.Images, "Test2.png");

        ImageData oldImage = await this.CreateFileAsync<ImageData>(
            FileTypeEnum.USER_IMAGE, 
            InitConst.MockAuthUserId, 
            GetResourceTestFile(ResourceProvider.Images, "Test1.png"),
            CancellationToken.None);
        
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {            
            var handler = new UpdateImageCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMongoDbRepository<ImageData>>(),
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );
            
            // Act
            await TestUtilities.Handle_InvalidCommand<UpdateImageCommand, MappedHelperResponse<ImageDataResponse, ImageData>, LoggerException>(
                handler, 
                new UpdateImageCommand(oldImage.Id, file), 
                ErrorMessages.Forbidden,
                async () =>
                {
                    await this.DeleteFileAsync<ImageData>(m => m.Id == oldImage.Id, CancellationToken.None);
                });
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUserImageQueryForNonAdminWithDifferentUserId_ReturnsException()
    {
        // Arrange
        ImageData image = await this.CreateFileAsync<ImageData>(
            FileTypeEnum.USER_IMAGE, 
            InitConst.MockAuthUserId, 
            GetResourceTestFile(ResourceProvider.Images, "Test1.png"),
            CancellationToken.None);

        UserImageQuery query = new UserImageQuery(image.Id)
        {
            Type = FileTypeEnum.USER_IMAGE,
            UserId = InitConst.MockAuthUserId
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
            await TestUtilities.Handle_InvalidCommand<UserImageQuery, MappedHelperResponse<ImageDataResponse, ImageData>, LoggerException>(
                handler, 
                query, 
                ErrorMessages.Forbidden,
                async () =>
                {
                    await this.DeleteFileAsync<ImageData>(m => m.Id == image.Id, CancellationToken.None);
                });
        }
    }
    
    [Test]
    public async Task Handle_ForbiddenUsersImagesQueryForNonAdminWithDifferentUserId_ReturnsException()
    {
        // Arrange
        UsersImagesQuery query = new UsersImagesQuery
        {
            Ids = new BaseIdsListQuery
            {
                Ids = new List<Guid> { InitConst.MockAuthUserId, InitConst.MockUserId, InitConst.MockAdminUserId }
            },
            Type = FileTypeEnum.USER_IMAGE
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
            await TestUtilities.Handle_InvalidCommand<UsersImagesQuery, ListWithIncludeHelper<ImageDataResponse>, LoggerException>(
                handler, 
                query, 
                ErrorMessages.Forbidden);
        }
    }
}