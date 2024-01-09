namespace CIYW.GraphQL.MutationResolver;

public interface IGraphQLMutationResolver
{
    void CreateInvoice();
    void UpdateInvoice();
    void DeleteInvoice();
}