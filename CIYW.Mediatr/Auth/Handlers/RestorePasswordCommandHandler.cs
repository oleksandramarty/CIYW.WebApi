using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class RestorePasswordCommandHandler: IRequestHandler<RestorePasswordCommand>
{
    public async Task Handle(RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}