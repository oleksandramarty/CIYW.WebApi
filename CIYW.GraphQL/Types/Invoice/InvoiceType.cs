using CIYW.Repositories;
using GraphQL.Types;
using Humanizer;

namespace CIYW.GraphQL.Types.Invoice;

public class InvoiceType: ObjectGraphType<Domain.Models.Invoice.Invoice>
{
    public InvoiceType()
    {
        Field(x => x.Id);
        Field(x => x.Name);
        Field(x => x.Amount);
        Field(x => x.UserId);
        // Field(x => x.User, true);
        Field(x => x.CurrencyId);
        // Field(x => x.Currency, true);
        Field(x => x.CategoryId);
        // Field(x => x.Category, true);
        Field(x => x.NoteId, true);
        // Field(x => x.Note, true);
        Field(x => x.Type);
        Field<StringGraphType>("modified", resolve: context => (context.Source.Updated ?? context.Source.Created).Humanize());
        Field<StringGraphType>("date", resolve: context => (context.Source.Date).Humanize());
        //Field<IntGraphType>("applicantCount", resolve: context => context.Source.JobApplicants.Count);
    }
}