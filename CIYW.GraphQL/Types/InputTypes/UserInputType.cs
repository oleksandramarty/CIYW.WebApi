using CIYW.Const.Enums;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.InputTypes;

public class UserInputType: InputObjectGraphType
{
    public UserInputType()
    {
        Name = "UserInput";
        Field<NonNullGraphType<StringGraphType>>("lastName");
        Field<NonNullGraphType<StringGraphType>>("firstName");
        Field<NonNullGraphType<StringGraphType>>("patronymic");
        Field<NonNullGraphType<StringGraphType>>("login");
        Field<NonNullGraphType<StringGraphType>>("email");
        Field<NonNullGraphType<StringGraphType>>("phone");
        Field<NonNullGraphType<StringGraphType>>("confirmEmail");
        Field<NonNullGraphType<BooleanGraphType>>("isAgree");
        Field<NonNullGraphType<StringGraphType>>("password");
        Field<NonNullGraphType<StringGraphType>>("confirmPassword");
        Field<NonNullGraphType<BooleanGraphType>>("isAgreeDigest");
        Field<NonNullGraphType<BooleanGraphType>>("isTemporaryPassword");
        Field<NonNullGraphType<BooleanGraphType>>("isBlocked").DefaultValue(false);
        Field<NonNullGraphType<IdGraphType>>("tariffId");
        Field<NonNullGraphType<IdGraphType>>("currencyId");
    }
}