using CIYW.Mediator.Auth.Queries;
using MediatR;

namespace CIYW.Mediator.Auth.Handlers;

public class ResetPasswordCheckAccessQueryHandler: IRequestHandler<ResetPasswordCheckAccessQuery>
{
    public async Task Handle(ResetPasswordCheckAccessQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}