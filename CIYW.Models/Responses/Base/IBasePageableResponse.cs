namespace CIYW.Models.Responses.Base;

public interface IBasePageableResponse
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
    int TotalCount { get; set; }
}