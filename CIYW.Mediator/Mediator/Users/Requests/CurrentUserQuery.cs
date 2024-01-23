using CIYW.Domain.Models.Users;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class CurrentUserQuery: IRequest<MappedHelperResponse<UserResponse, User>>
{
}