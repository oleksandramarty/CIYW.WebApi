using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Requests;

public class UserImageQuery: BaseQuery, IRequest<ImageDataResponse>
{
    public UserImageQuery(Guid id)
    {
        Id = id;
    }
}