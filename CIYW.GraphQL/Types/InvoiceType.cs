using CIYW.Kernel.Extensions;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

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
        Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
        Field(x => x.CategoryId);
        Field<CategoryType>("category", resolve: context => context.Source.Category);
        Field(x => x.NoteId, true);
        // Field(x => x.Note, true);
        Field(x => x.Type);
        Field<StringGraphType>("modified", resolve: context => context.Source.HumanizeModified());
        Field<StringGraphType>("date", resolve: context => (context.Source.Date).Humanize());
        //Field<IntGraphType>("applicantCount", resolve: context => context.Source.JobApplicants.Count);
    }
}