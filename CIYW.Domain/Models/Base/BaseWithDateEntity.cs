namespace CIYW.Domain.Models;

public class BaseWithDateEntity: BaseEntity
{
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}