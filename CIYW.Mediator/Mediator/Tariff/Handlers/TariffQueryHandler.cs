using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Handlers;

public class TariffQueryHandler: UserEntityValidatorHelper, IRequestHandler<TariffQuery, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;
    private readonly IEntityValidator entityValidator;

    public TariffQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository, 
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
        this.entityValidator = entityValidator;
    }

    public async Task<MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>> Handle(TariffQuery request, CancellationToken cancellationToken)
    {
        Domain.Models.Tariff.Tariff tariff = await this.tariffRepository.GetByIdAsync(request.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Tariff.Tariff, Guid?>(tariff, request.Id);

        var response = this.GetMappedHelper<TariffResponse, Domain.Models.Tariff.Tariff>(tariff);

        return response;
    }
}