using CIYW.GraphQL.Types;
using CIYW.Interfaces;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.QueryResolver;

public class GraphQLQueryResolver: ObjectGraphType, IGraphQLQueryResolver
{
    public void GetCurrencyById()
    {
        Field<CurrencyType>("currency")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Currency.Currency>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                return await repository.GetByIdAsync(entityId, cancellationToken);
            });
    }

    public void GetInvoiceById()
    {
        Field<InvoiceType>("invoice")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Invoice.Invoice>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                return await repository.GetWithIncludeAsync(u => u.Id == entityId, cancellationToken,
                    query => query.Include(u => u.Currency),
                    query => query.Include(u => u.Category));
            });
    }
    public void GetCategoryById()
    {
        Field<InvoiceType>("category")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid entityId = context.GetArgument<Guid>("id", default);
                var repository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Category.Category>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                return await repository.GetByIdAsync(entityId, cancellationToken);
            });
    }
}