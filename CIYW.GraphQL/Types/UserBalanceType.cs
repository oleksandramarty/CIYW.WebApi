using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class UserBalanceType: ObjectGraphType<Domain.Models.User.UserBalance>
{
    public UserBalanceType()
    {
        Field(x => x.Id);
        Field(x => x.Amount);
        Field(x => x.UserId);
        Field<UserType>("user", resolve: context => context.Source.User);
        Field(x => x.CurrencyId);
        Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
    }
}