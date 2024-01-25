using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Requests;

public class ChangePasswordCommand: IRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmationPassword { get; set; }
}