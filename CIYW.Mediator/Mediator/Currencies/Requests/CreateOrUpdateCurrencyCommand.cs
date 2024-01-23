using CIYW.Domain.Models.Currencies;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Currencies;
using MediatR;

namespace CIYW.Mediator.Mediator.Currencies.Requests;

public class CreateOrUpdateCurrencyCommand: BaseNullableQuery, IRequest<MappedHelperResponse<CurrencyResponse, Currency>>
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    
    public bool IsActive { get; set; }
}