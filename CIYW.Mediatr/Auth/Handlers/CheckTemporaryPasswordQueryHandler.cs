using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class CheckTemporaryPasswordQueryHandler: IRequestHandler<CheckTemporaryPasswordQuery, bool>
{
    public async Task<bool> Handle(CheckTemporaryPasswordQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}