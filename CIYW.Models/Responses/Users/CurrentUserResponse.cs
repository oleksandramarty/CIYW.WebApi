namespace CIYW.Models.Responses.Users;

public class CurrentUserResponse
{
    public Guid Id { get; set; }
    
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }

    public bool IsTemporaryPassword { get; set; }
    public bool Need2FAuthentication { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    
    public Guid RoleId { get; set; }
    public string Role { get; set; }
}