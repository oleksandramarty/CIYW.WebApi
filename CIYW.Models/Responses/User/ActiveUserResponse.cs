using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Users;

public class ActiveUserResponse: BaseWithDateEntityResponse
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public string Groups { get; set; }
}