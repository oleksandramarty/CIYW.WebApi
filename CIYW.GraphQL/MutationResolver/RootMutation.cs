using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.GraphQL.Types;
using CIYW.GraphQL.Types.InputTypes;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Mediator.Mediator.Note.Request;
using GraphQL.Types;

namespace CIYW.GraphQL.MutationResolver;

public class RootMutation: GraphQLMutationResolver
{
    public RootMutation()
    {
        Name = "Mutation";
        
        this.CreateEntity<InvoiceType, InvoiceInputType, CreateInvoiceCommand, Invoice>("createInvoice");
        this.UpdateEntity<InvoiceType, InvoiceInputType, UpdateInvoiceCommand, Invoice, GuidGraphType>("updateInvoice");
        this.DeleteEntity<DeleteInvoiceCommand, GuidGraphType>("deleteInvoice");
        
        this.CreateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, Note>("createNote");
        this.UpdateEntity<NoteType, NoteInputType, CreateOrUpdateNoteCommand, Note, GuidGraphType>("updateNote");
        this.DeleteEntity<DeleteNoteCommand, GuidGraphType>("deleteNote");
    }
}