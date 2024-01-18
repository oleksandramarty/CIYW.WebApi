using AutoMapper;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Currency.Handlers;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Models.Responses.Currency;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Currency;

[TestFixture]
public class CurrencyQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidCurrencyQuery_ReturnsCurrencyResponse()
    {
        // Arrange
        var query = new CurrencyQuery(InitConst.CurrencyUsdId);
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            var handler = new CurrencyQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currency.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>()
            );

            // Act
            MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Entity.Id.Should().Be(InitConst.CurrencyUsdId);
        }
    }
}