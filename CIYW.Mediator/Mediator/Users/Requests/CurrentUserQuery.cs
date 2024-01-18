using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class CurrentUserQuery: IRequest<UserResponse>
{
}