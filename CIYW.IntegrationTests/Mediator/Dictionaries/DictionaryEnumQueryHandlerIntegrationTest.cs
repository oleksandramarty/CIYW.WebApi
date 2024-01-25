using CIYW.Const.Enums;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Dictionaries.Handlers;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using FluentAssertions;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Mediator.Dictionaries;

[TestFixture]
public class DictionaryEnumQueryHandlerIntegrationTest: CommonIntegrationTestSetup
{
    [TestCase(DictionaryTypeEnum.INVOICE_TYPE)]
    public async Task Handle_ValidDictionaryEnumQuery_ReturnsDictionaries(DictionaryTypeEnum entityType)
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
            case DictionaryTypeEnum.INVOICE_TYPE:
                result.Items.Count.Should()
                    .Be(EnumExtension.ConvertEnumToDictionary<InvoiceTypeEnum>().Items.Count);
                return;
        }
    }
}