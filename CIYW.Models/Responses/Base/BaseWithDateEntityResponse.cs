namespace CIYW.Models.Responses.Base;

public class BaseWithDateEntityResponse: BaseEntityResponse
{
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}