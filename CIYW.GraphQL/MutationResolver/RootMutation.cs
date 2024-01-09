using GraphQL.Types;

namespace CIYW.GraphQL.MutationResolver;

public class RootMutation: GraphQLMutationResolver
{
    public RootMutation()
    {
        Name = "Mutation";
        this.CreateInvoice();
        this.UpdateInvoice();
        this.DeleteInvoice();
    }
}