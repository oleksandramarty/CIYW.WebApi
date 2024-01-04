using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Category.Handlers;
using CIYW.Mediator.Mediator.Category.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Category;

[TestFixture]
public class CreateCategoryCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateCreateCategoryTestCases()
    {   
        for (int i = 1; i <= 10; i++)
        {
            yield return new TestCaseData($"Category {i} Name", $"Category {i} Description", $"Category {i} Ico");
        }
    }
    
    [Test, TestCaseSource(nameof(CreateCreateCategoryTestCases))]
    public async Task Handle_ValidCreateOrUpdateCategoryCommand_ReturnsCategoryId(string name, string description, string ico)
    {
        // Arrange
        CreateOrUpdateCategoryCommand command = new CreateOrUpdateCategoryCommand
        {
            Name = name,
            Description = description,
            Ico = ico,
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new CreateCategoryCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Category.Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            Guid result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Categories.Count(c => c.Id == result && c.IsActive == true).Should().Be(1);
        }
    }
}