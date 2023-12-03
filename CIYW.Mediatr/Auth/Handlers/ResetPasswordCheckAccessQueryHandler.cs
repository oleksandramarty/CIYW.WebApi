using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class ResetPasswordCheckAccessQueryHandler: IRequestHandler<ResetPasswordCheckAccessQuery>
{
    public async Task Handle(ResetPasswordCheckAccessQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}