using CIYW.Mediator.Auth.Queries;
using MediatR;

namespace CIYW.Mediator.Auth.Handlers;

public class CheckTemporaryPasswordQueryHandler: IRequestHandler<CheckTemporaryPasswordQuery, bool>
{
    public async Task<bool> Handle(CheckTemporaryPasswordQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}