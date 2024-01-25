using CIYW.Const.Errors;
using CIYW.Domain.Models.Tariffs;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Categories.Requests;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Models.Responses.Tariffs;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class CreateOrUpdateTariffCommandValidatorTest
{
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Name = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Name));
        string req_Description = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Description));
        string req_Id = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateTariffCommand.Id));
        string maxLen_Name = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateTariffCommand.Name), 50);
        string maxLen_Description = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateTariffCommand.Description), 500);
        
        yield return new TestCaseData(1, true, null, StringExtension.GenerateRandomString(49), StringExtension.GenerateRandomString(499), null);
        yield return new TestCaseData(2, false, Guid.NewGuid(), StringExtension.GenerateRandomString(49), StringExtension.GenerateRandomString(499), null);
        yield return new TestCaseData(3, true, null, StringExtension.GenerateRandomString(50), StringExtension.GenerateRandomString(500), null);
        yield return new TestCaseData(4, false, Guid.NewGuid(), StringExtension.GenerateRandomString(50), StringExtension.GenerateRandomString(500), null);
        
        yield return new TestCaseData(5, false, null, StringExtension.GenerateRandomString(51), StringExtension.GenerateRandomString(501), new string[] { req_Id, maxLen_Name, maxLen_Description });
        yield return new TestCaseData(6, false, null, null, null, new string[] { req_Id, req_Name, req_Description });
        
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(int number, bool isNew, Guid? id, string name, string description,  string[]? expectedErrors)
    {
        // Arrange
        CreateOrUpdateTariffCommand command = new CreateOrUpdateTariffCommand
        {
            Id = id,
            Name = name,
            Description = description,
        };
        
        // Act
        TestUtilities.Validate_Command<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Tariff>>(
            command, () => new CreateOrUpdateTariffCommandValidator(isNew), expectedErrors);
    }
}