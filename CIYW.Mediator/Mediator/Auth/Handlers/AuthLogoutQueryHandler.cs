using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class AuthLogoutQueryHandler: IRequestHandler<AuthLogoutQuery>
{
    private readonly IAuthRepository authRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public AuthLogoutQueryHandler(
        IAuthRepository authRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        this.authRepository = authRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task Handle(AuthLogoutQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        await this.authRepository.LogOutUserAsync(userId, cancellationToken);
    }
}