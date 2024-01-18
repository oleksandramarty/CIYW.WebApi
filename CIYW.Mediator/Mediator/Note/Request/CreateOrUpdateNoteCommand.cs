using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Note;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Request;

public class CreateOrUpdateNoteCommand: BaseNullableQuery, IRequest<MappedHelperResponse<NoteResponse, Domain.Models.Note.Note>>
{
    public string Name { get; set; }
    public string Body { get; set; }
}