namespace CIYW.GraphQL.QueryResolver;

public interface IGraphQLQueryResolver
{
    void GetCurrencyById();
    void GetInvoiceById();
    void GetCategoryById();
    void GetUserById();
    void GetUserByIdForAdmin();
    void GetUserBalanceByUserId();
    void GetInvoiceHistory();
    void GetUsers();
}