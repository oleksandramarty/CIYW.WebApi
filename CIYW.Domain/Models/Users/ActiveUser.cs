namespace CIYW.Domain.Models.Users;

public class ActiveUser: BaseWithDateEntity
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public string Groups { get; set; }
}