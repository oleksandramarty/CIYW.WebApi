using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Validators.Currencies;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

public class CreateOrUpdateCurrencyCommandValidatorTest
{
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Name = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Name));
        string req_Symbol = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Symbol));
        string req_IsoCode = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.IsoCode));
        string req_Id = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCurrencyCommand.Id));
        string maxLen_Name = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCurrencyCommand.Name), 50);
        string maxLen_Symbol = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCurrencyCommand.Symbol), 3);
        string exactLen_IsoCode = String.Format(ErrorMessages.FieldExactLengthError, nameof(CreateOrUpdateCurrencyCommand.IsoCode), 3);
        
        yield return new TestCaseData(1, true, null, StringExtension.GenerateRandomString(49), "USD", "US", null);
        yield return new TestCaseData(2, false, Guid.NewGuid(), StringExtension.GenerateRandomString(49), "USD", "US", null);
        yield return new TestCaseData(3, true, null, StringExtension.GenerateRandomString(50), "USD", "USD", null);
        yield return new TestCaseData(4, false, Guid.NewGuid(), StringExtension.GenerateRandomString(50), "USD", "USD", null);
        
        yield return new TestCaseData(5, false, null, StringExtension.GenerateRandomString(51), "USDA", "USDA", new string[] { req_Id, maxLen_Name, exactLen_IsoCode, maxLen_Symbol });
        yield return new TestCaseData(6, false, null, null, null, null, new string[] { req_Id, req_Name, req_IsoCode, req_Symbol });
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(int number, bool isNew, Guid? id, string name, string isoCode, string symbol,  string[]? expectedErrors)
    {
        // Arrange
        CreateOrUpdateCurrencyCommand command = new CreateOrUpdateCurrencyCommand
        {
            Id = id,
            Name = name,
            IsoCode = isoCode,
            Symbol = symbol
        };
        
        // Act
        TestUtilities.Validate_Command<CreateOrUpdateCurrencyCommand, Guid>(
            command, () => new CreateOrUpdateCurrencyCommandValidator(isNew), expectedErrors);
    }
}