using CIYW.GraphQL.Queries.Invoice;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.GraphQL.Schemas.Invoice;

public class InvoiceSchema: Schema
{
    public InvoiceSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<InvoiceQuery>();
        // Mutation = serviceProvider.GetRequiredService<NotesMutation>();
    }
}