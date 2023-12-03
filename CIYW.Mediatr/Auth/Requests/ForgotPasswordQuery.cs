using MediatR;

namespace CIYW.Mediatr.Auth.Queries;

public class ForgotPasswordQuery: IRequest
{
    public ForgotPasswordQuery(string login, string email, string phone)
    {
        Login = login ?? throw new ArgumentNullException(nameof(login));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
    }

    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}