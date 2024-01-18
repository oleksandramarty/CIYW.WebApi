using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Models.Responses.Users;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class UserType: ObjectGraphType<UserResponse>
{
    public UserType()
    {
        Field(x => x.Id);
        Field(x => x.Login);
        Field(x => x.LastName);
        Field(x => x.FirstName);
        Field(x => x.Patronymic);
        Field(x => x.IsTemporaryPassword);
        Field(x => x.IsBlocked);
        Field(x => x.RoleId);
        Field<StringGraphType>("modified", resolve: context => (context.Source.Updated ?? context.Source.Created).Humanize());
        Field<StringGraphType>("lastForgot", resolve: context => context.Source.LastForgot.Humanize());
        Field(x => x.TariffId);
        // Field(x => x.Tariff);
        Field(x => x.CurrencyId);
        Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
        Field(x => x.UserBalanceId);
        Field<UserBalanceType>("userBalance", resolve: context => context.Source.UserBalance);
        Field(x => x.Email);
        Field(x => x.Created);
        Field(x => x.Updated, true);
        Field(x => x.EmailConfirmed);
        Field(x => x.PhoneNumber);
        Field(x => x.PhoneNumberConfirmed);
    }
}