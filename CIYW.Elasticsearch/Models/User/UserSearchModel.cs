namespace CIYW.Elasticsearch.Models.User;

public class UserSearchModel
{
    public Guid Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsBlocked { get; set; }
    public Guid RoleId { get; set; }
    public Guid TariffId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public UserBalanceSearchModel UserBalance { get; set; }
}