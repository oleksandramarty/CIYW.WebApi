using CIYW.Mediator.Mediatr.Auth.Requests;
using MediatR;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class ChangePasswordCommandHandler: IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}