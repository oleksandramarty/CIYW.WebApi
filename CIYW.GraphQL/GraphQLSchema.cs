using CIYW.GraphQL.MutationResolver;
using CIYW.GraphQL.QueryResolver;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL;

public class GraphQLSchema: Schema
{
    public GraphQLSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<RootQuery>();
        Mutation = serviceProvider.GetRequiredService<RootMutation>();
    }
}