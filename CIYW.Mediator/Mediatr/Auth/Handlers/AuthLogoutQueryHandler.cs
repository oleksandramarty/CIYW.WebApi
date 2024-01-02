using CIYW.Interfaces;
using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class AuthLogoutQueryHandler: IRequestHandler<AuthLogoutQuery>
{
    private readonly IAuthRepository _authRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AuthLogoutQueryHandler(
        IAuthRepository authRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        _authRepository = authRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task Handle(AuthLogoutQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this._currentUserProvider.GetUserIdAsync(cancellationToken);
        await this._authRepository.LogOutUserAsync(userId, cancellationToken);
    }
}