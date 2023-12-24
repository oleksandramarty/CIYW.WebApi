using MediatR;

namespace CIYW.Mediator.Auth.Queries;

public class RestorePasswordCommand: IRequest
{
    public RestorePasswordCommand(string login, string email, string password, string confirmPassword, string url, string phone)
    {
        Login = login ?? throw new ArgumentNullException(nameof(login));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        ConfirmPassword = confirmPassword ?? throw new ArgumentNullException(nameof(confirmPassword));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
    }

    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Url { get; set; }
}