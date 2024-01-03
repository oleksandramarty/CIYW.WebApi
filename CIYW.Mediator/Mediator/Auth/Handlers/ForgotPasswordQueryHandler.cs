using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class ForgotPasswordQueryHandler: IRequestHandler<ForgotPasswordQuery>
{
    public async Task Handle(ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}