using CIYW.Const.Errors;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Validators.Auth;
using CIYW.TestHelper;
using NUnit.Framework;

namespace CIYW.IntegrationTests.Validators;

[TestFixture]
public class ForgotPasswordQueryValidatorTest
{
        private static IEnumerable<TestCaseData> CreateTestCases()
    {
        string req_Login = String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Login));
        string req_Email = String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Email));
        string req_Phone = String.Format(ErrorMessages.FieldIsRequired, nameof(ForgotPasswordQuery.Phone));

        string login = StringExtension.GenerateRandomString(10);
        string email = StringExtension.GenerateRandomString(15);
        string phone = StringExtension.GenerateRandomString(11);
        
        yield return new TestCaseData(1, login, email, phone, null);
        yield return new TestCaseData(2, null, email, phone, new string[] { req_Login });
        yield return new TestCaseData(3, login, null, phone, new string[] { req_Email });
        yield return new TestCaseData(4, login, email, null, new string[] { req_Phone });
        yield return new TestCaseData(5, null, null, phone, new string[] { req_Login, req_Email });
        yield return new TestCaseData(6, login, null, null, new string[] { req_Email, req_Phone });
        yield return new TestCaseData(7, null, email, null, new string[] { req_Login, req_Phone });
        yield return new TestCaseData(8, null, null, null, new string[] { req_Login, req_Email, req_Phone });
    }
    
    [Test, TestCaseSource(nameof(CreateTestCases))]
    public async Task Handle_Query_ValidateResult(int number, string login, string email, string phone, string[]? expectedErrors)
    {
        // Arrange
        ForgotPasswordQuery command = new ForgotPasswordQuery
        {
            Login = login,
            Email = email,
            Phone = phone,
        };
        
        // Act
        TestUtilities.Validate_Command<ForgotPasswordQuery>(
            command, () => new ForgotPasswordQueryValidator(), expectedErrors);
    }
}