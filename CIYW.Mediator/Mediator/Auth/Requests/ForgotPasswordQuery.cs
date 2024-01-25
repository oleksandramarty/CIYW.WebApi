using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Requests;

public class ForgotPasswordQuery: IRequest
{
    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}