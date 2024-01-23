using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Categories;
using CIYW.Interfaces;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Categories.Handlers;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Models.Responses.Categories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Categories;

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
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<CategoryResponse, Category> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Categories.Count(c => c.Id == result.Entity.Id && c.IsActive == true).Should().Be(1);
        }
    }
}