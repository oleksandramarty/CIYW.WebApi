using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Note.Request;

public class DeleteNoteCommand: BaseQuery, IRequest
{
    public DeleteNoteCommand(Guid id)
    {
        Id = id;
    }
}