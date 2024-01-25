using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Requests;

public class RestorePasswordCommand: IRequest
{
    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Url { get; set; }
}