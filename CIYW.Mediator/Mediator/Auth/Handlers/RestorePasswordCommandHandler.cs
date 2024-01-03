using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class RestorePasswordCommandHandler: IRequestHandler<RestorePasswordCommand>
{
    public async Task Handle(RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}