using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediatr.Note.Request;

public class DeleteNoteCommand: BaseQuery, IRequest
{
    public DeleteNoteCommand(Guid id)
    {
        Id = id;
    }
}