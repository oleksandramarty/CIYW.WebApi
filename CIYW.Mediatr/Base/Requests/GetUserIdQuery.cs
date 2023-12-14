using System.Security.Claims;
using MediatR;

namespace CIYW.Mediatr.Base.Requests;

public class GetUserIdQuery: IRequest<Guid>
{
}