namespace CIYW.Models.Requests.Common;

public class BaseFilterQuery
{
    public BaseIdsListQuery? Ids { get; set; }
    public Paginator? Paginator { get; set; }
    public BaseDateRangeQuery? DateRange { get; set; }
    public BaseSortableQuery? Sort { get; set; }
}

public class BaseSortableQuery
{
    public string ParentClass { get; set; }
    public string Column { get; set; }
    public string Direction { get; set; }
}

public class Paginator
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

