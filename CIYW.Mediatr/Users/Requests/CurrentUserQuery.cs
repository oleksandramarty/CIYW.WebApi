using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediatr.Users.Requests;

public class CurrentUserQuery: IRequest<CurrentUserResponse>
{
    public CurrentUserQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}