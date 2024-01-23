using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Notes;
using CIYW.Domain.Models.Users;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.InputTypes;
using CIYW.Mediator;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Mediator.Mediator.Notes.Request;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Invoices;
using CIYW.Models.Responses.Notes;
using CIYW.Models.Responses.Users;
using GraphQL.Types;

namespace CIYW.GraphQL.MutationResolver;

public class RootMutation: GraphQLMutationResolver
{
    public RootMutation()
    {
        Name = "Mutation";
        
        this.CreateEntity<InvoiceType, InvoiceInputType, CreateInvoiceCommand, InvoiceResponse, Invoice>("createInvoice");
        this.UpdateEntity<InvoiceType, InvoiceInputType, UpdateInvoiceCommand, InvoiceResponse, Invoice, GuidGraphType>("updateInvoice");
        this.DeleteEntity<DeleteInvoiceCommand, GuidGraphType>("deleteInvoice");
        
        this.CreateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, NoteResponse, Note>("createNote");
        this.UpdateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, NoteResponse, Note, GuidGraphType>("updateNote");
        this.DeleteEntity<DeleteNoteCommand, GuidGraphType>("deleteNote");
        
        this.CreateEntity<UserType, UserInputType, CreateUserByAdminCommand, UserResponse, User>("createUserByAdmin");
        this.UpdateEntity<UserType, UserInputType, UpdateUserByAdminCommand, UserResponse, User, GuidGraphType>("updateUserByAdmin");
    }
}