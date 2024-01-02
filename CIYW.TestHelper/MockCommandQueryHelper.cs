using CIYW.Mediator.Mediatr.Note.Request;
using CIYW.Mediator.Mediatr.Users.Requests;

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
            "11223344556",
            "email@mail.com",
            true,
            "Password123",
            "Password123",
            false);
    }

    public static CreateOrUpdateNoteCommand CreateNoteCommand(Guid? id = null)
    {
        return new CreateOrUpdateNoteCommand
        {
            Id = id,
            Name = "Name",
            Body = "Body"
        };
    }
}