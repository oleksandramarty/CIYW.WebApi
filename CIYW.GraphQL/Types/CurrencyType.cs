using CIYW.Models.Responses.Currency;
using GraphQL.Types;

namespace CIYW.GraphQL.Types;

public class CurrencyType: ObjectGraphType<CurrencyResponse>
{
    public CurrencyType()
    {
        Field(x => x.Id);
        Field(x => x.Name);
        Field(x => x.IsoCode);
        Field(x => x.Symbol);
        Field(x => x.IsActive);
    }
}