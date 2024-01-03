using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Requests;

public class ChangePasswordCommand: IRequest
{
    public ChangePasswordCommand(string oldPassword, string newPassword, string confirmationPassword)
    {
        OldPassword = oldPassword ?? throw new ArgumentNullException(nameof(oldPassword));
        NewPassword = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        ConfirmationPassword = confirmationPassword ?? throw new ArgumentNullException(nameof(confirmationPassword));
    }

    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmationPassword { get; set; }
}