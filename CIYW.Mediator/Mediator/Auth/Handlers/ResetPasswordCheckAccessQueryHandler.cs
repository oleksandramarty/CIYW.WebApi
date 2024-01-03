using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class ResetPasswordCheckAccessQueryHandler: IRequestHandler<ResetPasswordCheckAccessQuery>
{
    public async Task Handle(ResetPasswordCheckAccessQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}