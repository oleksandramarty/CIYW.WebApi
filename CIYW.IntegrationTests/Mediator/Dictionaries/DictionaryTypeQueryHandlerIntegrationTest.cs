using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Dictionaries.Handlers;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Dictionaries;

[TestFixture]
public class DictionaryTypeQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [TestCase(DictionaryTypeEnum.TARIFF)]
    [TestCase(DictionaryTypeEnum.CATEGORY)]
    [TestCase(DictionaryTypeEnum.CURRENCY)]
    [TestCase(DictionaryTypeEnum.ROLE)]
    public async Task Handle_ValidDictionaryTypeQuery_ReturnsDictionaries(DictionaryTypeEnum entityType)
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
                case DictionaryTypeEnum.CURRENCY:
                    dbContext.Currencies.Count().Should().Be(result.Items.Count);
                    return;
                case DictionaryTypeEnum.CATEGORY:
                    dbContext.Categories.Count().Should().Be(result.Items.Count);
                    return;
                case DictionaryTypeEnum.TARIFF:
                    dbContext.Tariffs.Count().Should().Be(result.Items.Count);
                    return;
                case DictionaryTypeEnum.ROLE:
                    dbContext.Roles.Count().Should().Be(result.Items.Count);
                    return;
            }
        }
    }
}