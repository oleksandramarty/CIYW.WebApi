using CIYW.Interfaces;
using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class AuthLogoutQueryHandler: IRequestHandler<AuthLogoutQuery>
{
    private readonly IAuthRepository _authRepository;

    public AuthLogoutQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task Handle(AuthLogoutQuery query, CancellationToken cancellationToken)
    {
        // query.UserId = Get User Id
        await this._authRepository.LogOutUserAsync(query.UserId, cancellationToken);
    }
}