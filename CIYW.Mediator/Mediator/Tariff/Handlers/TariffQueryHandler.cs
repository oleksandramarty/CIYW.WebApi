using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Handlers;

public class TariffQueryHandler: IRequestHandler<TariffQuery, TariffResponse>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;
    private readonly IEntityValidator entityValidator;

    public TariffQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
        this.entityValidator = entityValidator;
    }

    public async Task<TariffResponse> Handle(TariffQuery request, CancellationToken cancellationToken)
    {
        Domain.Models.Tariff.Tariff tariff = await this.tariffRepository.GetByIdAsync(request.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Tariff.Tariff, Guid?>(tariff, request.Id);

        TariffResponse response = this.mapper.Map<Domain.Models.Tariff.Tariff, TariffResponse>(tariff);

        return response;
    }
}