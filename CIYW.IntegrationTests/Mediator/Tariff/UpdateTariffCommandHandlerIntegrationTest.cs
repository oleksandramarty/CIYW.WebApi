using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Tariff.Handlers;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Tariff;

[TestFixture]
public class UpdateTariffCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUpdateTariffTestCases()
    {   
        for (int i = 1; i <= 10; i++)
        {
            yield return new TestCaseData($"Updated Tariff {i} Name", $"Updated Tariff {i} Description", BoolExtension.GetRandomBool());
        }
    }
    
    [Test, TestCaseSource(nameof(CreateUpdateTariffTestCases))]
    public async Task Handle_ValidCreateOrUpdateTariffCommand_ReturnsTariffId(string name, string description, bool isActive)
    {
        // Arrange
        CreateOrUpdateTariffCommand command = new CreateOrUpdateTariffCommand
        {
            Name = name,
            Description = description,
            IsActive = isActive
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            Domain.Models.Tariff.Tariff tariff = dbContext.Tariffs.FirstOrDefault();
            command.Id = tariff.Id;
            
            var handler = new UpdateTariffCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Tariff.Tariff>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Tariffs.Count(
                c => c.Id == result.Entity.Id &&
                     c.Name == name &&
                     c.Description == description &&
                     c.IsActive == isActive).Should().Be(1);
        }
    }
}