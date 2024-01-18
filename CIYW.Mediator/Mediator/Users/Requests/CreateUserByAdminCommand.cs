namespace CIYW.Mediator.Mediator.Users.Requests;

public class CreateUserByAdminCommand: CreateUserCommand
{
    public CreateUserByAdminCommand(
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
        Guid currencyId): base(lastName, firstName, patronymic, login, email, phone, confirmEmail, isAgree, password, confirmPassword, isAgreeDigest)
    {
        IsTemporaryPassword = isTemporaryPassword;
        IsBlocked = isBlocked;
        TariffId = tariffId;
        CurrencyId = currencyId;
    }
    
    public bool IsTemporaryPassword { get; set; }
    public bool IsBlocked { get; set; } = false;
    public Guid TariffId { get; set; }
    public Guid CurrencyId { get; set; }
}