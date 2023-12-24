using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Tariff.Handlers;

public class TariffQueryHandler: IRequestHandler<TariffQuery, TariffResponse>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;

    public TariffQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
    }

    public async Task<TariffResponse> Handle(TariffQuery request, CancellationToken cancellationToken)
    {
        Domain.Models.Tariff.Tariff tariff = await this.tariffRepository.GetByIdAsync(request.Id, cancellationToken);

        TariffResponse response = this.mapper.Map<Domain.Models.Tariff.Tariff, TariffResponse>(tariff);

        return response;
    }
}