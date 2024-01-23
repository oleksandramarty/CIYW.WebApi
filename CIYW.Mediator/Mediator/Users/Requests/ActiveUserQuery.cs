using CIYW.Domain.Models.Users;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class ActiveUserQuery: IRequest<MappedHelperResponse<ActiveUserResponse, ActiveUser>>
{
    
}