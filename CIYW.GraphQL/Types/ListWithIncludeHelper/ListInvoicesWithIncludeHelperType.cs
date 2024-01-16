using CIYW.Domain.Models.Invoice;
using CIYW.Models.Helpers.Base;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.ListWithIncludeHelper;

public class ListInvoicesWithIncludeHelperType: ObjectGraphType<ListWithIncludeHelper<Invoice>> 
{
    public ListInvoicesWithIncludeHelperType()
    {
        Field<ListGraphType<InvoiceType>>("entities", resolve: context => context.Source.Entities);
        Field<PaginatorType>("paginator", resolve: context => context.Source.Paginator);
        Field(x => x.TotalCount);
    }
}