namespace CIYW.Models.Helpers.Base;

public class ListWithIncludeHelper<T>
{
    public IList<T> Entities { get; set; }
    public int Total { get; set; }
}