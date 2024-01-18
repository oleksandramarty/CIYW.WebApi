using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Base;

namespace CIYW.Models.Helpers.Base;

public class ListWithIncludeHelper<T>
{
    public IList<T> Entities { get; set; }
    public Paginator Paginator { get; set; }
    public long TotalCount { get; set; }
}