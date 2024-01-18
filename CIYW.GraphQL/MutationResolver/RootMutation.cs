using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.InputTypes;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using CIYW.Models.Responses.Invoice;
using CIYW.Models.Responses.Note;
using GraphQL.Types;

namespace CIYW.GraphQL.MutationResolver;

public class RootMutation: GraphQLMutationResolver
{
    public RootMutation()
    {
        Name = "Mutation";
        
        this.CreateEntity<InvoiceType, InvoiceInputType, CreateInvoiceCommand, InvoiceResponse>("createInvoice");
        this.UpdateEntity<InvoiceType, InvoiceInputType, UpdateInvoiceCommand, InvoiceResponse, GuidGraphType>("updateInvoice");
        this.DeleteEntity<DeleteInvoiceCommand, GuidGraphType>("deleteInvoice");
        
        this.CreateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, NoteResponse>("createNote");
        this.UpdateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, NoteResponse, GuidGraphType>("updateNote");
        this.DeleteEntity<DeleteNoteCommand, GuidGraphType>("deleteNote");
    }
}