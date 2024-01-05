using CIYW.GraphQL.Types.Invoice;
using CIYW.Interfaces;
using CIYW.Repositories;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.Queries.Invoice;

public class InvoiceQuery: ObjectGraphType
{
    public InvoiceQuery()
    {
        Field<InvoiceType>("invoice")
            .Arguments(new QueryArguments(new QueryArgument<GuidGraphType> { Name = "id" }))
            .ResolveAsync(async context =>
            {
                Guid invoiceId = context.GetArgument<Guid>("id", default);
                var invoiceRepository = context.RequestServices.GetRequiredService<IReadGenericRepository<Domain.Models.Invoice.Invoice>>();
                
                // Assuming cancellationToken is available in your GraphQL context
                var cancellationToken = context.CancellationToken;

                return await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
            });
    }
}  