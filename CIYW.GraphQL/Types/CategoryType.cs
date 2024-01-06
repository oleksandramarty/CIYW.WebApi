using CIYW.Kernel.Extensions;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class CategoryType: ObjectGraphType<Domain.Models.Category.Category>
{
    public CategoryType()
    {
        Field(x => x.Id);
        Field(x => x.Name);
        Field(x => x.Description);
        Field(x => x.Ico);
        Field(x => x.IsActive);
        Field<StringGraphType>("modified", resolve: context => context.Source.HumanizeModified());
    }
}