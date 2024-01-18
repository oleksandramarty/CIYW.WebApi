using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Note;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Request;

public class CreateOrUpdateNoteCommand: BaseNullableQuery, IRequest<NoteResponse>
{
    public string Name { get; set; }
    public string Body { get; set; }
}