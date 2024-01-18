using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Category.Handlers;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Models.Responses.Category;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Category;

[TestFixture]
public class UpdateCategoryCommandHandlerIntegrationTest() : CommonIntegrationTestSetup(InitConst.MockAdminUserId)
{
    private static IEnumerable<TestCaseData> CreateUpdateCategoryTestCases()
    {   
        for (int i = 1; i <= 10; i++)
        {
            yield return new TestCaseData($"Updated Category {i} Name", $"Updated Category {i} Description", $"Updated Category {i} Ico", BoolExtension.GetRandomBool());
        }
    }
    
    [Test, TestCaseSource(nameof(CreateUpdateCategoryTestCases))]
    public async Task Handle_ValidCreateOrUpdateCategoryCommand_ReturnsCategoryId(string name, string description, string ico, bool isActive)
    {
        // Arrange
        CreateOrUpdateCategoryCommand command = new CreateOrUpdateCategoryCommand
        {
            Name = name,
            Description = description,
            Ico = ico,
            IsActive = isActive
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            Domain.Models.Category.Category category = dbContext.Categories.FirstOrDefault();
            command.Id = category.Id;
            
            var handler = new UpdateCategoryCommandHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IGenericRepository<Domain.Models.Category.Category>>(),
                scope.ServiceProvider.GetRequiredService<IEntityValidator>(),
                scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()
            );

            // Act
            MappedHelperResponse<CategoryResponse, Domain.Models.Category.Category> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            dbContext.Categories.Count(
                c => c.Id == result.Entity.Id &&
                     c.Name == name &&
                     c.Description == description &&
                     c.Ico == ico &&
                     c.IsActive == isActive).Should().Be(1);
        }
    }
}