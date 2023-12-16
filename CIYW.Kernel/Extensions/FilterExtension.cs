using CIYW.Models.Requests.Common;

namespace CIYW.Kernel.Extensions;

public static class FilterExtension
{
    public static BaseFilterQuery CreateDefaultFilter(string? dateColumn = null)
    {
        return new BaseFilterQuery
        {
            Ids = null,

            Paginator = new BasePageableQuery
            {
                PageNumber = 1,
                PageSize = 10,
                IsFull = false
            },

            DateRange = new BaseDateRangeQuery
            {
                DateFrom = DateTimeExtension.GetStartOfTheMonth(),
                DateTo = DateTimeExtension.GetEndOfTheMonth()
            }
        };
    }
}