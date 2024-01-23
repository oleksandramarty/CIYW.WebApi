using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class DeleteImageCommand: BaseQuery, IRequest
{
    public DeleteImageCommand(Guid id)
    {
        Id = id;
    }
}