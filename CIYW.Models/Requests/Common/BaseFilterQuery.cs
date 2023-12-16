namespace CIYW.Models.Requests.Common;

public class BaseFilterQuery
{
    public BaseIdsListQuery? Ids { get; set; }
    public BasePageableQuery? Paginator { get; set; }
    public BaseDateRangeQuery? DateRange { get; set; }
}

public class BasePageableQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IsFull { get; set; } = false;
}

public class BaseDateRangeQuery
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class BaseIdsListQuery
{
    public IList<Guid> Ids { get; set; } = new List<Guid>();
}

