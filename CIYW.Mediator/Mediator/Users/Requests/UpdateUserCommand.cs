namespace CIYW.Mediator.Mediator.Users.Requests;

public class UpdateUserCommand: CreateUserCommand
{
    public UpdateUserCommand(
        string lastName, 
        string firstName, 
        string patronymic, 
        string login, 
        string email, 
        string phone, 
        string confirmEmail, 
        bool isAgree, 
        string password, 
        string confirmPassword, 
        bool isAgreeDigest) : base(lastName, firstName, patronymic, login, email, phone, confirmEmail, isAgree, password, confirmPassword, isAgreeDigest)
    {
    }
}