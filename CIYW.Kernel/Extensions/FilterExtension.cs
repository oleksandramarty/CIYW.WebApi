using CIYW.Models.Requests.Common;

namespace CIYW.Kernel.Extensions;

public static class FilterExtension
{
    public static BaseFilterQuery CreateDefaultFilter(string? dateColumn = null, Paginator? paginator = null)
    {
        return new BaseFilterQuery
        {
            Ids = null,

            Paginator = paginator ?? new Paginator
            {
                PageNumber = 1,
                PageSize = 5,
                IsFull = false
            },

            DateRange = new BaseDateRangeQuery
            {
                DateFrom = DateTimeExtension.GetStartOfTheMonth(),
                DateTo = DateTimeExtension.GetEndOfTheMonth()
            },
            
            Sort = null
        };
    }
}