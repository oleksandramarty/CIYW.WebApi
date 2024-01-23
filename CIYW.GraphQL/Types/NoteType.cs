using CIYW.Mediator;
using CIYW.Models.Responses.Notes;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class NoteType: ObjectGraphType<NoteResponse>
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