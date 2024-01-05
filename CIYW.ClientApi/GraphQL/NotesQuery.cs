using CIYW.Domain.Models.Note;
using GraphQL.Types;

namespace CIYW.ClientApi.GraphQL;

public class NotesQuery: ObjectGraphType
{
    public NotesQuery()
    {
        Field<ListGraphType<NoteType>>("notes", resolve: context => new List<Note> {
            new Note { Id = Guid.NewGuid(), Name = "Hello World!", Body = "Body"},
            new Note { Id = Guid.NewGuid(), Name = "Hello World! How are you?", Body = "Body 2" }
        });
    }
}