using CIYW.Models.Responses.Users;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class UserBalanceType: ObjectGraphType<UserBalanceResponse>
{
    public UserBalanceType()
    {
        Field(x => x.Id);
        Field(x => x.Amount);
        Field(x => x.UserId);
        Field(x => x.CurrencyId);
        Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
    }
}