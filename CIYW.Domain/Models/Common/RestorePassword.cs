namespace CIYW.Domain.Models.Common;

public class RestorePassword: BaseWithDateEntity
{
    public Guid UserId { get; set; }
    public string Url { get; set; }
    public DateTime? Used { get; set; }
}