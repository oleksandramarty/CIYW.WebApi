﻿using CIYW.Domain.Models.User;
using CIYW.Models.Helpers.Base;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.ListWithIncludeHelper;

public class ListUsersWithIncludeHelperType: ObjectGraphType<ListWithIncludeHelper<User>> 
{
    public ListUsersWithIncludeHelperType()
    {
        Field<ListGraphType<UserType>>("entities", resolve: context => context.Source.Entities);
        Field<PaginatorType>("paginator", resolve: context => context.Source.Paginator);
        Field(x => x.TotalCount);
    }
}