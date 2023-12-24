using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Currency.Requests;

public class CurrencyQuery: BaseQuery, IRequest<CurrencyResponse>
{
    public CurrencyQuery(Guid id)
    {
        Id = id;
    }
}