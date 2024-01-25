using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Validators.Auth;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class RestorePasswordCommandValidatorTest
{
    private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Login = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Login));
        string req_Email = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Email));
        string req_Phone = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Phone));
        string req_Url = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Url));
        string req_Pas = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.Password));
        string req_PassConfirm = String.Format(ErrorMessages.FieldIsRequired, nameof(RestorePasswordCommand.ConfirmPassword));
        
        string minLen_Url = String.Format(ErrorMessages.FieldExactLengthError, nameof(RestorePasswordCommand.Url), 50);
        string maxLen_Url = String.Format(ErrorMessages.FieldExactLengthError, nameof(RestorePasswordCommand.Url), 50);
        string passMatch = ErrorMessages.PasswordsDoesntMatch;

        string login = StringExtension.GenerateRandomString(10);
        string email = StringExtension.GenerateRandomString(15);
        string phone = StringExtension.GenerateRandomString(11);
        string url = StringExtension.GenerateRandomString(50).ToUpper();
        string pass = StringExtension.GenerateRandomString(10);
        string conf = pass;
       
        yield return new TestCaseData(1, null, email, phone, pass, conf, url, new string[] { req_Login });
        yield return new TestCaseData(2, login, null, phone, pass, conf, url, new string[] { req_Email });
        yield return new TestCaseData(3, login, email, null, pass, conf, url, new string[] { req_Phone });
        yield return new TestCaseData(4, login, email, phone, null, conf, url, new string[] { req_Pas, passMatch });
        yield return new TestCaseData(5, login, email, phone, pass, null, url, new string[] { req_PassConfirm, req_Pas, passMatch });
        yield return new TestCaseData(6, login, email, phone, pass, conf, null, new string[] { req_Url });
        yield return new TestCaseData(7, null, null, phone, pass, conf, url, new string[] { req_Login, req_Email });
        yield return new TestCaseData(8, login, null, null, pass, conf, url, new string[] { req_Email, req_Phone });
        yield return new TestCaseData(9, login, email, null, null, conf, url, new string[] { req_Phone, req_Pas, passMatch });
        yield return new TestCaseData(10, login, email, phone, null, null, url, new string[] { req_Pas, passMatch, req_PassConfirm, req_Pas });
        yield return new TestCaseData(11, login, email, phone, pass, null, null, new string[] { req_PassConfirm, passMatch, req_Url });
        yield return new TestCaseData(12, null, email, phone, pass, conf, null, new string[] { req_Login, req_Url });
        yield return new TestCaseData(13, null, null, null, pass, conf, url, new string[] { req_Login, req_Email, req_Phone });
        yield return new TestCaseData(14, login, null, null, null, conf, url, new string[] { req_Email, req_Phone, req_Pas, passMatch });
        yield return new TestCaseData(15, login, email, null, null, null, url, new string[] { req_Phone, req_Pas, passMatch, req_PassConfirm, req_Pas });
        yield return new TestCaseData(16, login, email, phone, null, null, null, new string[] { req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(17, null, email, phone, pass, null, null, new string[] { req_Login, req_PassConfirm, passMatch, req_Url });
        yield return new TestCaseData(18, null, null, phone, pass, conf, null, new string[] { req_Login, req_Email, req_Url });
        yield return new TestCaseData(19, null, null, null, null, conf, url, new string[] { req_Login, req_Email, req_Phone, req_Pas, passMatch });
        yield return new TestCaseData(20, login, null, null, null, null, url, new string[] { req_Email, req_Phone, req_Pas, passMatch, req_PassConfirm, req_Pas });
        yield return new TestCaseData(21, login, email, null, null, null, null, new string[] { req_Phone, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(22, null, email, phone, null, null, null, new string[] { req_Login, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(23, null, null, phone, pass, null, null, new string[] { req_Login, req_Email, req_PassConfirm, passMatch, req_Url });
        yield return new TestCaseData(24, null, null, null, pass, conf, null, new string[] { req_Login, req_Email, req_Phone, req_Url });
        yield return new TestCaseData(25, null, null, null, null, null, url, new string[] { req_Login, req_Email, req_Phone, req_Pas, passMatch, req_PassConfirm, req_Pas });
        yield return new TestCaseData(26, login, null, null, null, null, null, new string[] { req_Email, req_Phone, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(27, null, email, null, null, null, null, new string[] { req_Login, req_Phone, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(28, null, null, phone, null, null, null, new string[] { req_Login, req_Email, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(29, null, null, null, pass, null, null, new string[] { req_Login, req_Email, req_Phone, req_PassConfirm, passMatch, req_Url });
        yield return new TestCaseData(30, null, null, null, null, conf, null, new string[] { req_Login, req_Email, req_Phone, req_Pas, passMatch, req_Url });
        yield return new TestCaseData(31, null, null, null, null, null, null, new string[] { req_Login, req_Email, req_Phone, req_Pas, passMatch, req_PassConfirm, req_Url });
        yield return new TestCaseData(32, login, email, phone, pass, StringExtension.GenerateRandomString(11), url, new string[] { passMatch });
        yield return new TestCaseData(33, login, email, phone, pass, conf, StringExtension.GenerateRandomString(49), new string[] { minLen_Url });
        yield return new TestCaseData(34, login, email, phone, pass, conf, StringExtension.GenerateRandomString(51), new string[] { maxLen_Url });
        yield return new TestCaseData(35, login, email, phone, pass, conf, url, null);
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(
        int number,
        string login,
        string email,
        string phone,
        string pass,
        string conf,
        string url,
        string[]? expectedErrors)
    {
        // Arrange
        RestorePasswordCommand command = new RestorePasswordCommand
        {
            Login = login,
            Email = email,
            Phone = phone,
            Password = pass,
            ConfirmPassword = conf,
            Url = url
        };
        
        // Act
        TestUtilities.Validate_Command<RestorePasswordCommand>(
            command, () => new RestorePasswordCommandValidator(), expectedErrors);
    }
}