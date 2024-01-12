using CIYW.Domain.Models.Note;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class NoteType: ObjectGraphType<Note>
{
    public NoteType()
    {
        Field(x => x.Id);
        Field(x => x.Name);
        Field(x => x.Body, true);
        Field(x => x.InvoiceId, true);
        Field(x => x.Created);
        Field(x => x.Updated, true);
    }
}