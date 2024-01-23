using AutoMapper;
using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Handlers;

public class TariffQueryHandler: UserEntityValidatorHelper, IRequestHandler<TariffQuery, MappedHelperResponse<TariffResponse, Tariff>>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Tariff> tariffRepository;
    private readonly IEntityValidator entityValidator;

    public TariffQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Tariff> tariffRepository, 
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
        this.entityValidator = entityValidator;
    }

    public async Task<MappedHelperResponse<TariffResponse, Tariff>> Handle(TariffQuery request, CancellationToken cancellationToken)
    {
        Tariff tariff = await this.tariffRepository.GetByIdAsync(request.Id, cancellationToken);

        this.entityValidator.ValidateExist<Tariff, Guid?>(tariff, request.Id);

        var response = this.GetMappedHelper<TariffResponse, Tariff>(tariff);

        return response;
    }
}