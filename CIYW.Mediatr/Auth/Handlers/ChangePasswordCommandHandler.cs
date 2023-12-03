using CIYW.Mediatr.Auth.Queries;
using MediatR;

namespace CIYW.Mediatr.Auth.Handlers;

public class ChangePasswordCommandHandler: IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}