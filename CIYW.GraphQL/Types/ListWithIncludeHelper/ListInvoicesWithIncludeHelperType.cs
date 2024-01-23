using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Invoices;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.ListWithIncludeHelper;

public class ListInvoicesWithIncludeHelperType: ObjectGraphType<ListWithIncludeHelper<InvoiceResponse>> 
{
    public ListInvoicesWithIncludeHelperType()
    {
        Field<ListGraphType<InvoiceType>>("entities", resolve: context => context.Source.Entities);
        Field<PaginatorType>("paginator", resolve: context => context.Source.Paginator);
        Field(x => x.TotalCount);
    }
}