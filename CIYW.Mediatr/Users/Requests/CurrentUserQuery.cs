using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediatr.Users.Requests;

public class CurrentUserQuery: IRequest<CurrentUserResponse>
{
}