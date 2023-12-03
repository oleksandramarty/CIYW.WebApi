using CIYW.Interfaces;
using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class AuthLogoutQueryHandler: IRequestHandler<AuthLogoutQuery>
{
    private readonly IAuthService _authService;

    public AuthLogoutQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task Handle(AuthLogoutQuery query, CancellationToken cancellationToken)
    {
        // query.UserId = Get User Id
        await this._authService.LogOutUserAsync(query.UserId, cancellationToken);
    }
}