using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Requests;

public class TariffsQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<TariffResponse>>
{
    
}