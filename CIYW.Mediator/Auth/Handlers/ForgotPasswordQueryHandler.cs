using CIYW.Mediator.Auth.Queries;
using MediatR;

namespace CIYW.Mediator.Auth.Handlers;

public class ForgotPasswordQueryHandler: IRequestHandler<ForgotPasswordQuery>
{
    public async Task Handle(ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}