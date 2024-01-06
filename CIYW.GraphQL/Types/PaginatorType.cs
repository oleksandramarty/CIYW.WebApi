using CIYW.Models.Requests.Common;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class PaginatorType: ObjectGraphType<Paginator>
{
    public PaginatorType()
    {
        Field(x => x.PageNumber);
        Field(x => x.PageSize);
        Field(x => x.IsFull);
    }
}