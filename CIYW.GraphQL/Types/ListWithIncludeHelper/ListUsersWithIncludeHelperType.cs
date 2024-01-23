using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Users;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.ListWithIncludeHelper;

public class ListUsersWithIncludeHelperType: ObjectGraphType<ListWithIncludeHelper<UserResponse>> 
{
    public ListUsersWithIncludeHelperType()
    {
        Field<ListGraphType<UserType>>("entities", resolve: context => context.Source.Entities);
        Field<PaginatorType>("paginator", resolve: context => context.Source.Paginator);
        Field(x => x.TotalCount);
    }
}