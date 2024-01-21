using CIYW.Domain.Models.User;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class ActiveUserQuery: IRequest<MappedHelperResponse<ActiveUserResponse, ActiveUser>>
{
    
}