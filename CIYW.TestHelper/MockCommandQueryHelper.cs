using CIYW.Mediator.Auth.Queries;

namespace CIYW.TestHelper;

public static class MockCommandQueryHelper
{
    public static CreateUserCommand CreateCreateUserCommand()
    {
        return new CreateUserCommand(
            "LastName",
            "FirstName",
            "Test",
            "Login",
            "email@mail.com",
            "12124567890",
            "email@mail.com",
            true,
            "Password123",
            "Password123",
            false);
    }
}