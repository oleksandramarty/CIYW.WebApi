using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediatr.Users.Requests;

public class CurrentUserQuery: IRequest<CurrentUserResponse>
{
}