using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class ForgotPasswordQueryHandler: IRequestHandler<ForgotPasswordQuery>
{
    public async Task Handle(ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}