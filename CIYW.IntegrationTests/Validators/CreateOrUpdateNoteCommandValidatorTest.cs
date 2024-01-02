using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Kernel.Extensions.Validators.Note;
using CIYW.Mediator.Mediatr.Note.Request;
using CIYW.TestHelper;
using CIYW.UnitTests;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class CreateOrUpdateNoteCommandValidatorTest
{
    [Test]
    public async Task Handle_InvalidQuery_NameLengthException()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Name = StringExtension.GenerateRandomString(51);
        string[] expectedErrors = { String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Name), 50) };
        
        // Act
        TestUtilities.Validate_InvalidCommand<CreateOrUpdateNoteCommand, Guid>(
            command, () => new CreateOrUpdateNoteCommandValidator(true), expectedErrors);
    }
    
    [Test]
    public async Task Handle_InvalidQuery_NameRequiredException()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Name = null;
        string[] expectedErrors = { String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Name)) };
        
        // Act
        TestUtilities.Validate_InvalidCommand<CreateOrUpdateNoteCommand, Guid>(
            command, () => new CreateOrUpdateNoteCommandValidator(true), expectedErrors);
    }
    
    [Test]
    public async Task Handle_InvalidQuery_BodyLengthException()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Body = StringExtension.GenerateRandomString(501);
        string[] expectedErrors = { String.Format(ErrorMessages.FieldMaxLengthError, nameof(CreateOrUpdateNoteCommand.Body), 500) };
        
        // Act
        TestUtilities.Validate_InvalidCommand<CreateOrUpdateNoteCommand, Guid>(
            command, () => new CreateOrUpdateNoteCommandValidator(true), expectedErrors);
    }
    
    [Test]
    public async Task Handle_InvalidQuery_BodyRequiredException()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand();
        command.Body = null;
        string[] expectedErrors = { String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Body)) };
        
        // Act
        TestUtilities.Validate_InvalidCommand<CreateOrUpdateNoteCommand, Guid>(
            command, () => new CreateOrUpdateNoteCommandValidator(true), expectedErrors);
    }
    
    [Test]
    public async Task Handle_InvalidQuery_IdForUpdateRequiredException()
    {
        // Arrange
        CreateOrUpdateNoteCommand command = MockCommandQueryHelper.CreateNoteCommand(null);
        string[] expectedErrors = { String.Format(ErrorMessages.FieldIsRequired, nameof(CreateOrUpdateNoteCommand.Id)) };
        
        // Act
        TestUtilities.Validate_InvalidCommand<CreateOrUpdateNoteCommand, Guid>(
            command, () => new CreateOrUpdateNoteCommandValidator(false), expectedErrors);
    }
}