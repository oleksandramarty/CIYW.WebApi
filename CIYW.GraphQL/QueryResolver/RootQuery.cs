using CIYW.Repositories;

namespace CIYW.GraphQL.QueryResolver;

public class RootQuery: GraphQLQueryResolver
{
    public RootQuery()
    {
        this.GetCurrencyById();
        this.GetInvoiceById();
        this.GetCategoryById();
    }
}  