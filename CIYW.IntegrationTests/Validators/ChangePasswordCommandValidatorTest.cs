using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Validators.Auth;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

public class ChangePasswordCommandValidatorTest
{
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_OldLogin = String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.OldPassword));
        string req_NewLogin = String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.NewPassword));
        string req_Conf = String.Format(ErrorMessages.FieldIsRequired, nameof(ChangePasswordCommand.ConfirmationPassword));

        string passMatch = ErrorMessages.PasswordsDoesntMatch;

        string oldPas = StringExtension.GenerateRandomString(10);
        string newPas = StringExtension.GenerateRandomString(10);
        string conf = newPas;
       
        yield return new TestCaseData(1, oldPas, newPas, conf, null);
        yield return new TestCaseData(2, null, newPas, conf, new string[] { req_OldLogin });
        yield return new TestCaseData(3, oldPas, null, conf, new string[] { req_NewLogin, passMatch });
        yield return new TestCaseData(4, oldPas, newPas, null, new string[] { req_Conf, passMatch });
        yield return new TestCaseData(5, null, null, conf, new string[] { req_OldLogin, req_NewLogin, passMatch });
        yield return new TestCaseData(6, oldPas, null, null, new string[] { req_NewLogin, passMatch, req_Conf });
        yield return new TestCaseData(7, null, newPas, null, new string[] { req_OldLogin, req_Conf, passMatch });
        yield return new TestCaseData(8, null, null, null, new string[] { req_OldLogin, req_NewLogin, passMatch, req_Conf });
        yield return new TestCaseData(9, oldPas, newPas, StringExtension.GenerateRandomString(11), new string[] { passMatch });
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(
        int number,
        string oldPas,
        string newPas,
        string conf,
        string[]? expectedErrors)
    {
        // Arrange
        ChangePasswordCommand command = new ChangePasswordCommand
        {
            OldPassword = oldPas,
            NewPassword = newPas,
            ConfirmationPassword = conf
        };
        
        // Act
        TestUtilities.Validate_Command<ChangePasswordCommand>(
            command, () => new ChangePasswordCommandValidator(), expectedErrors);
    }
}