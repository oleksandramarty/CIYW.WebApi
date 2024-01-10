using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Dictionary.Handlers;
using CIYW.Mediator.Mediator.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Dictionary;

[TestFixture]
public class DictionaryTypeQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [TestCase(EntityTypeEnum.Tariff)]
    [TestCase(EntityTypeEnum.Category)]
    [TestCase(EntityTypeEnum.Currency)]
    [TestCase(EntityTypeEnum.Role)]
    public async Task Handle_ValidDictionaryTypeQuery_ReturnsDictionaries(EntityTypeEnum entityType)
    {
        // Arrange
        DictionaryTypeQuery query = new DictionaryTypeQuery(entityType);

        using (var scope = this.testApplicationFactory.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var handler = new DictionaryTypeQueryHandler(
                scope.ServiceProvider.GetRequiredService<IMapper>(),
                scope.ServiceProvider.GetRequiredService<IDictionaryRepository>());

            // Act
            DictionaryResponse<Guid> result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            switch (entityType)
            {
                case EntityTypeEnum.Currency:
                    dbContext.Currencies.Count().Should().Be(result.Items.Count);
                    return;
                case EntityTypeEnum.Category:
                    dbContext.Categories.Count().Should().Be(result.Items.Count);
                    return;
                case EntityTypeEnum.Tariff:
                    dbContext.Tariffs.Count().Should().Be(result.Items.Count);
                    return;
                case EntityTypeEnum.Role:
                    dbContext.Roles.Count().Should().Be(result.Items.Count);
                    return;
            }
        }
    }
}