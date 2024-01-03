using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class CheckTemporaryPasswordQueryHandler: IRequestHandler<CheckTemporaryPasswordQuery, bool>
{
    public async Task<bool> Handle(CheckTemporaryPasswordQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}