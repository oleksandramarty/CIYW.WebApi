using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class CheckTemporaryPasswordQueryHandler: IRequestHandler<CheckTemporaryPasswordQuery, bool>
{
    public async Task<bool> Handle(CheckTemporaryPasswordQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}