using CIYW.Domain;
using CIYW.Mediator.Mediator.Dictionaries.Handlers;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Dictionaries;

[TestFixture]
public class DictionaryQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [Test]
    public async Task Handle_ValidDictionaryQuery_ReturnsDictionaries()
    {
        // Arrange
        DictionaryQuery query = new DictionaryQuery();
            
        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            var handler = new DictionaryQueryHandler(scope.ServiceProvider.GetRequiredService<IMediator>());

            // Act
            DictionariesResponse result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Currencies.Should().NotBeNull();
            result.Categories.Should().NotBeNull();
            result.Tariffs.Should().NotBeNull();
            result.Roles.Should().NotBeNull();
            dbContext.Currencies.Count().Should().Be(result.Currencies.Items.Count);
            dbContext.Categories.Count().Should().Be(result.Categories.Items.Count);
            dbContext.Tariffs.Count().Should().Be(result.Tariffs.Items.Count);
            dbContext.Roles.Count().Should().Be(result.Roles.Items.Count);
        }
    }
}