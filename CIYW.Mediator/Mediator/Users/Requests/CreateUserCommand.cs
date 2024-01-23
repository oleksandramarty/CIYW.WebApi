using CIYW.Domain.Models.Users;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Requests;

public class CreateUserCommand: IRequest<MappedHelperResponse<UserResponse, User>>
{
    public CreateUserCommand(string lastName, string firstName, string patronymic, string login, string email, string phone, string confirmEmail, bool isAgree, string password,
        string confirmPassword, bool isAgreeDigest)
    {
        Login = login ?? throw new ArgumentNullException(nameof(login));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        ConfirmEmail = confirmEmail ?? throw new ArgumentNullException(nameof(confirmEmail));
        IsAgree = isAgree;
        Password = password ?? throw new ArgumentNullException(nameof(password));
        ConfirmPassword = confirmPassword ?? throw new ArgumentNullException(nameof(confirmPassword));
        IsAgreeDigest = isAgreeDigest;
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        Patronymic = patronymic;
    }
    
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ConfirmEmail { get; set; }
    public bool IsAgree { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool IsAgreeDigest { get; set; }
}