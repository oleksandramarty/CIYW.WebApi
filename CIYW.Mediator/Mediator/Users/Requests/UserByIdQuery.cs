using CIYW.Domain.Models.User;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class UserByIdQuery: BaseQuery, IRequest<MappedHelperResponse<UserResponse, User>>
{
    public UserByIdQuery(Guid id)
    {
        Id = id;
    }
}