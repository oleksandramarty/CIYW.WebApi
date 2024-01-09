using CIYW.Const.Errors;
using CIYW.Domain.Models.Note;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Mediator.Validators.Notes;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class CreateOrUpdateNoteCommandValidatorTest
{    
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Name = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Name));
        string req_Body = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Body));
        string req_Id = String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Id));
        string maxLen_Name = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Name), 50);
        string maxLen_Body = String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Body), 500);
        
        yield return new TestCaseData(1, true, null, StringExtension.GenerateRandomString(49), StringExtension.GenerateRandomString(499), null);
        yield return new TestCaseData(2, false, Guid.NewGuid(), StringExtension.GenerateRandomString(49), StringExtension.GenerateRandomString(499), null);
        yield return new TestCaseData(3, true, null, StringExtension.GenerateRandomString(50), StringExtension.GenerateRandomString(500), null);
        yield return new TestCaseData(4, false, Guid.NewGuid(), StringExtension.GenerateRandomString(50), StringExtension.GenerateRandomString(500), null);
        
        yield return new TestCaseData(5, false, null, StringExtension.GenerateRandomString(51), StringExtension.GenerateRandomString(501), new string[] { req_Id, maxLen_Name, maxLen_Body });
        yield return new TestCaseData(6, false, null, null, null, new string[] { req_Id, req_Name, req_Body });
        
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(int number, bool isNew, Guid? id, string name, string body,  string[]? expectedErrors)
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Id = id;
        command.Name = name;
        command.Body = body;
        
        // Act
        TestUtilities.Validate_Command<CreateOrUpdateNoteCommand, Note>(
            command, () => new CreateOrUpdateNoteCommandValidator(isNew), expectedErrors);
    }
}