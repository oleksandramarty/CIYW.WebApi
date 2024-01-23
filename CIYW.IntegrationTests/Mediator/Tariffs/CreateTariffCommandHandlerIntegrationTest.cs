using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Tariffs.Handlers;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Responses.Tariffs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Tariffs;

[TestFixture]
public class CreateTariffCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateCreateTariffTestCases()
    {   
        for (int i = 1; i <= 10; i++)
        {
            yield return new TestCaseData($"Tariff {i} Name", $"Tariff {i} Description");
        }
    }
    
    [Test, TestCaseSource(nameof(CreateCreateTariffTestCases))]
    public async Task Handle_ValidCreateOrUpdateTariffCommand_ReturnsTariffId(string name, string description)
    {
        // Arrange
        CreateOrUpdateTariffCommand command = new CreateOrUpdateTariffCommand
        {
            Name = name,
            Description = description
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new CreateTariffCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariffs.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<TariffResponse, Domain.Models.Tariffs.Tariff> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Tariffs.Count(c => c.Id == result.Entity.Id && c.IsActive == true).Should().Be(1);
        }
    }
}