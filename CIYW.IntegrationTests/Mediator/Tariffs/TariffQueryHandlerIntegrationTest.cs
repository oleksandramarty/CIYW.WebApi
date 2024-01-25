using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Tariffs.Handlers;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Responses.Tariffs;
using CIYW.TestHelper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Tariffs;

[TestFixture]
public class TariffQueryHandlerIntegrationTest : CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateValidTariffQueryTestCases()
    {   
        yield return new TestCaseData(InitConst.FreeTariffId);
    }
    
    [Test, TestCaseSource(nameof(CreateValidTariffQueryTestCases))]
    public async Task Handle_ValidTariffQuery_ReturnsTariff(Guid tariffId)
    {
        // Arrange
        TariffQuery query = new TariffQuery(tariffId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new TariffQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariffs.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<TariffResponse, Domain.Models.Tariffs.Tariff> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Entity.Should().NotBeNull();
            result.MappedEntity.Should().NotBeNull();
            result.Entity.Id.Should().Be(tariffId);
            result.MappedEntity.Id.Should().Be(tariffId);
        }
    }
    
    private static IEnumerable<TestCaseData> CreateInvalidTariffQueryTestCases()
    {
        for (int i = 1; i <= 5; i++)
        {
            yield return new TestCaseData(Guid.NewGuid());            
        }
    }
    
    [Test, TestCaseSource(nameof(CreateInvalidTariffQueryTestCases))]
    public async Task Handle_InvalidTariffQuery_ReturnsException(Guid tariffId)
    {
        // Arrange
        TariffQuery query = new TariffQuery(tariffId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new TariffQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariffs.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            await TestUtilities.Handle_InvalidCommand<TariffQuery, MappedHelperResponse<TariffResponse, Tariff>, LoggerException>(
                handler, 
                query, 
                String.Format(
                    ErrorMessages.EntityWithIdNotFound,
                    nameof(Tariff), 
                    null));
        }
    }
}