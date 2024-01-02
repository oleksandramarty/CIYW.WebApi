using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class ResetPasswordCheckAccessQueryHandler: IRequestHandler<ResetPasswordCheckAccessQuery>
{
    public async Task Handle(ResetPasswordCheckAccessQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}