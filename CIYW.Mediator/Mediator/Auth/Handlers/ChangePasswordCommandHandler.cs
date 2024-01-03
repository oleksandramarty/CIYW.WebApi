using CIYW.Mediator.Mediator.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class ChangePasswordCommandHandler: IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}