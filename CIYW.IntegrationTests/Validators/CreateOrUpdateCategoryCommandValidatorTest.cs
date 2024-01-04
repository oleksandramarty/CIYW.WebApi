using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Category.Requests;
using CIYW.Mediator.Validators.Categories;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class CreateOrUpdateCategoryCommandValidatorTest
{    
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Name = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Name));
        string req_Description = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Description));
        string req_Id = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateCategoryCommand.Id));
        string maxLen_Name = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCategoryCommand.Name), 50);
        string maxLen_Description = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateCategoryCommand.Description), 500);
        
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
        CreateOrUpdateCategoryCommand command = new CreateOrUpdateCategoryCommand
        {
            Id = id,
            Name = name,
            Description = description,
        };
        
        // Act
        TestUtilities.Validate_Command<CreateOrUpdateCategoryCommand, Guid>(
            command, () => new CreateOrUpdateCategoryCommandValidator(isNew), expectedErrors);
    }
}