using CIYW.Interfaces;
using CIYW.Mediatr.Base.Requests;
using MediatR;

namespace CIYW.Mediatr.Base.Handlers;

public class GetUserIdQueryHandler: IRequestHandler<GetUserIdQuery, Guid>
{
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetUserIdQueryHandler(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Guid> Handle(GetUserIdQuery query, CancellationToken cancellationToken)
    {
        return await this._currentUserProvider.GetUserIdAsync(cancellationToken);
    }
}