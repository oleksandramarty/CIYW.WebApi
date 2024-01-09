using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Request;

public class CreateOrUpdateNoteCommand: BaseNullableQuery, IRequest<Domain.Models.Note.Note>
{
    public string Name { get; set; }
    public string Body { get; set; }
}