using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class ForgotPasswordQueryHandler: IRequestHandler<ForgotPasswordQuery>
{
    public async Task Handle(ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}