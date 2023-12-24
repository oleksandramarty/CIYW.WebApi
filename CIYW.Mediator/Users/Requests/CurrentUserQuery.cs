using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Users.Requests;

public class CurrentUserQuery: IRequest<CurrentUserResponse>
{
}