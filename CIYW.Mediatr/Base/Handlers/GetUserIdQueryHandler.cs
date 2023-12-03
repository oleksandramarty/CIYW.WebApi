using CIYW.Interfaces;
using CIYW.Mediatr.Base.Requests;
using MediatR;

namespace CIYW.Mediatr.Base.Handlers;

public class GetUserIdQueryHandler: IRequestHandler<GetUserIdQuery, Guid>
{
    private readonly IAuthService _authService;

    public GetUserIdQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Guid> Handle(GetUserIdQuery query, CancellationToken cancellationToken)
    {
        return await this._authService.GetUserIdAsync(query.User, cancellationToken);
    }
}