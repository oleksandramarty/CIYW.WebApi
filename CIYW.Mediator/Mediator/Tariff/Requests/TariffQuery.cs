using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Requests;

public class TariffQuery: IRequest<MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>
{
    public TariffQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}