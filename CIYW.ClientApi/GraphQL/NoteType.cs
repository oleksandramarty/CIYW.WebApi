using CIYW.Domain.Models.Note;
using GraphQL.Types;

namespace CIYW.ClientApi.GraphQL;

public class NoteType : ObjectGraphType<Note>
{
    public NoteType()
    {
        Name = "Note";
        Description = "Note Type";
        Field(d => d.Id, nullable: false).Description("Note Id");
        Field(d => d.Name, nullable: true).Description("Note Name");
        Field(d => d.Body, nullable: true).Description("Note Body");
    }
}