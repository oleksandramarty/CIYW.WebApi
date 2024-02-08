using CIYW.Domain;
using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Tariffs.Handlers;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Tariffs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Tariffs;

[TestFixture]
public class TariffsQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    private static IEnumerable<TestCaseData> CreateTariffsQueryHandlerTestCases()
    {   
        yield return new TestCaseData(1, 5, false);
        yield return new TestCaseData(2, 5, false);
        yield return new TestCaseData(1, 1, false);
        yield return new TestCaseData(2, 1, false);
        yield return new TestCaseData(3, 1, false);
        yield return new TestCaseData(4, 1, false);
        yield return new TestCaseData(5, 1, false);
        yield return new TestCaseData(1, 1, true);
    }
    
    [Test, TestCaseSource(nameof(CreateTariffsQueryHandlerTestCases))]
    public async Task Handle_ValidTariffsQuery_ReturnsTariffs(int pageNumber, int pageSize, bool isFull)
    {
        // Arrange
        TariffsQuery query = new TariffsQuery
        {
            Paginator = new Paginator
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IsFull = isFull
            }
        };
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new TariffsQueryHandler(
                scope.ServiceProvider.GetRequiredService<IReadGenericRepository<Tariff>>()
            );

            // Act
            ListWithIncludeHelper<TariffResponse> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Entities.Should().NotBeNull();
            
            var totalCount = dbContext.Tariffs.Count();
            var listCount = isFull ? 
                totalCount : 
                dbContext.Tariffs.Skip((pageNumber - 1) * pageSize).Take(pageSize).Count();
            result.Entities.Count.Should().Be(listCount);
            result.TotalCount.Should().Be((long)totalCount);
        }
    }
}