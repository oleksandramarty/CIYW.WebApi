using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class RestorePasswordCommandHandler: IRequestHandler<RestorePasswordCommand>
{
    public async Task Handle(RestorePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}