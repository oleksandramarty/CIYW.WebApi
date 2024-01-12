using CIYW.Const.Enums;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Dictionary.Handlers;
using CIYW.Mediator.Mediator.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using FluentAssertions;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Dictionary;

[TestFixture]
public class DictionaryEnumQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [TestCase(EntityTypeEnum.INVOICE_TYPE)]
    public async Task Handle_ValidDictionaryEnumQuery_ReturnsDictionaries(EntityTypeEnum entityType)
    {
        // Arrange
        DictionaryEnumQuery query = new DictionaryEnumQuery(entityType);

        var handler = new DictionaryEnumQueryHandler();
        // Act
        DictionaryResponse<string> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        switch (entityType)
        {
            case EntityTypeEnum.INVOICE_TYPE:
                result.Items.Count.Should()
                    .Be(EnumExtension.ConvertEnumToDictionary<InvoiceTypeEnum>().Items.Count);
                return;
        }
    }
}