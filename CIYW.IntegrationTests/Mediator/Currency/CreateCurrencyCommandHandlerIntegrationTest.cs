using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Currency.Handlers;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Models.Responses.Currency;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Currency;

[TestFixture]
public class CreateCurrencyCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateCreateCurrencyTestCases()
    {   
        for (int i = 1; i <= 10; i++)
        {
            yield return new TestCaseData($"Currency {i} Name", StringExtension.GenerateRandomString(3), $"{i}");
        }
    }
    
    [Test, TestCaseSource(nameof(CreateCreateCurrencyTestCases))]
    public async Task Handle_ValidCreateOrUpdateCurrencyCommand_ReturnsCurrencyId(string name, string isoCode, string symbol)
    {
        // Arrange
        CreateOrUpdateCurrencyCommand command = new CreateOrUpdateCurrencyCommand
        {
            Name = name,
            IsoCode = isoCode,
            Symbol = symbol,
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new CreateCurrencyCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Currency.Currency>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Currencies.Count(c => c.Id == result.Entity.Id && c.IsActive == true).Should().Be(1);
        }
    }
}