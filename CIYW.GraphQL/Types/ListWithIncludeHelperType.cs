using CIYW.Models.Helpers.Base;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class ListWithIncludeHelperType<T, TType>: ObjectGraphType<ListWithIncludeHelper<T>> 
    where T: class
    where TType: ObjectGraphType<T>
{
    public ListWithIncludeHelperType()
    {
        Field<ListGraphType<TType>>("entities", resolve: context => context.Source.Entities);
        Field<PaginatorType>("paginator", resolve: context => context.Source.Paginator);
        Field(x => x.TotalCount);
    }
}