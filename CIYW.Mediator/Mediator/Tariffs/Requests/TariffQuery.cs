using CIYW.Domain.Models.Tariffs;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Requests;

public class TariffQuery: IRequest<MappedHelperResponse<TariffResponse, Tariff>>
{
    public TariffQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}