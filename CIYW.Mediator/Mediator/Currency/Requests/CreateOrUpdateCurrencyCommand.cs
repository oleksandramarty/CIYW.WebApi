using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Mediator.Currency.Requests;

public class CreateOrUpdateCurrencyCommand: BaseNullableQuery, IRequest<MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    
    public bool IsActive { get; set; }
}