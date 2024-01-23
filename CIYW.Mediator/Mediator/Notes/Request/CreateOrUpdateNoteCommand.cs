using CIYW.Domain.Models.Notes;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Notes;
using MediatR;

namespace CIYW.Mediator.Mediator.Notes.Request;

public class CreateOrUpdateNoteCommand: BaseNullableQuery, IRequest<MappedHelperResponse<NoteResponse, Note>>
{
    public string Name { get; set; }
    public string Body { get; set; }
}