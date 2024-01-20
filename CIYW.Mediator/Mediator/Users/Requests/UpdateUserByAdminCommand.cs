namespace CIYW.Mediator.Mediator.Users.Requests;

public class UpdateUserByAdminCommand: CreateUserByAdminCommand
{
    public UpdateUserByAdminCommand(
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
        bool isAgreeDigest, 
        bool isTemporaryPassword, 
        bool isBlocked, 
        Guid tariffId, 
        Guid currencyId) : base(lastName, firstName, patronymic, login, email, phone, confirmEmail, isAgree, password, confirmPassword, isAgreeDigest, isTemporaryPassword, isBlocked, tariffId, currencyId)
    {
    }
    
    public Guid Id { get; set; }
}