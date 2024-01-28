using CIYW.Const.Enums;
using CIYW.Mediator.Mediator.Auth.Queries;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Mediator.Mediator.Users.Requests;

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

    public static AuthLoginQuery CreateAuthLoginQuery()
    {
        return new AuthLoginQuery {
            Login = "anime.kit",
            Email = "animekit@mail.com", 
            Phone = "22334433221", 
            Password = "zcbm13579", 
            RememberMe = false
        };
    }

    public static CreateInvoiceCommand CreateCreateInvoiceCommand(
        decimal amount,
        Guid categoryId,
        Guid currencyId,
        DateTime date,
        InvoiceTypeEnum type,
        CreateOrUpdateNoteCommand? note = null)
    {
        return new CreateInvoiceCommand
        {
            Name = "TestInvoice",
            Amount = amount,
            CategoryId = categoryId,
            CurrencyId = currencyId,
            Date = date,
            Type = type,
            Note = note
        };
    }
    
    public static UpdateInvoiceCommand CreateUpdateInvoiceCommand(
        decimal amount,
        Guid categoryId,
        Guid currencyId,
        DateTime date,
        InvoiceTypeEnum type)
    {
        return new UpdateInvoiceCommand
        {
            Name = "TestInvoice",
            Amount = amount,
            CategoryId = categoryId,
            CurrencyId = currencyId,
            Date = date,
            Type = type,
        };
    }

    public static CreateOrUpdateNoteCommand CreateCreateOrUpdateNoteCommand(string name = "TestNoteName", string body = "TestNoteBody")
    {
        return new CreateOrUpdateNoteCommand
        {
            Name = name,
            Body = body
        };
    }
}