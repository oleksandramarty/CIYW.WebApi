using CIYW.Mediator.Auth.Queries;
using MediatR;

namespace CIYW.Mediator.Auth.Handlers;

public class RestorePasswordCommandHandler: IRequestHandler<RestorePasswordCommand>
{
    public async Task Handle(RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}