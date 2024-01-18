using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Category.Handlers;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Mediator.Mediator.Currency.Handlers;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Mediator.Tariff.Handlers;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Category;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Tariff;
using CIYW.TestHelper;
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Category.Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>, LoggerException>(handler, command, ErrorMessages.Forbidden);
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Category.Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCategoryCommand, MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category>, LoggerException>(handler, command, ErrorMessages.Forbidden);
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currency.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>, LoggerException>(handler, command, ErrorMessages.Forbidden);
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currency.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>, LoggerException>(handler, command, ErrorMessages.Forbidden);
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariff.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>, LoggerException>(handler, command, ErrorMessages.Forbidden);
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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariff.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>, LoggerException>(handler, command, ErrorMessages.Forbidden);
        }
    }
}