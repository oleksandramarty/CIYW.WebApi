using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Note.Request;

public class CreateOrUpdateNoteCommand: BaseNullableQuery, IRequest<Guid>
{
    public string Name { get; set; }
    public string Body { get; set; }
}