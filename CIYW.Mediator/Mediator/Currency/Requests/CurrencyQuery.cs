using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Mediator.Currency.Requests;

public class CurrencyQuery: BaseQuery, IRequest<MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>
{
    public CurrencyQuery(Guid id)
    {
        Id = id;
    }
}