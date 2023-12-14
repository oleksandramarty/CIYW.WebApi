using CIYW.Interfaces;
using CIYW.Mediatr.Base.Requests;
using MediatR;

namespace CIYW.Mediatr.Base.Handlers;

public class GetUserIdQueryHandler: IRequestHandler<GetUserIdQuery, Guid>
{
    private readonly IAuthRepository _authRepository;

    public GetUserIdQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Guid> Handle(GetUserIdQuery query, CancellationToken cancellationToken)
    {
        return await this._authRepository.GetUserIdAsync(query.User, cancellationToken);
    }
}