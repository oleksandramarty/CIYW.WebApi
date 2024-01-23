using CIYW.Domain.Models.Currencies;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Currencies;
using MediatR;

namespace CIYW.Mediator.Mediator.Currencies.Requests;

public class CurrencyQuery: BaseQuery, IRequest<MappedHelperResponse<CurrencyResponse, Currency>>
{
    public CurrencyQuery(Guid id)
    {
        Id = id;
    }
}