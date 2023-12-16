namespace CIYW.Models.Responses.Base;

public class BasePageableResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}