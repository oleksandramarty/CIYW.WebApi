using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Tariffs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Tariffs.Handlers;

public class TariffsQueryHandler: BasePageableHelper<Tariff>, IRequestHandler<TariffsQuery, ListWithIncludeHelper<TariffResponse>>
{
    public TariffsQueryHandler(
        IReadGenericRepository<Tariff> tariffRepository): base(tariffRepository)
    {
    }
    
    public async Task<ListWithIncludeHelper<TariffResponse>> Handle(TariffsQuery query, CancellationToken cancellationToken)
    {
        return await this.GetPageableResponseAsync<TariffResponse>(null,
            query,
            cancellationToken,
            q => 
                q.Include(q => q.CurrencyEntities)
                    .ThenInclude(q => q.Currency));
    }
}