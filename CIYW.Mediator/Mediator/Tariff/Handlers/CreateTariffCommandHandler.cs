using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Handlers;

public class CreateTariffCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;

    public CreateTariffCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
    }

    public async Task<MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>> Handle(CreateOrUpdateTariffCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>(command, () => new CreateOrUpdateTariffCommandValidator(true));
        Domain.Models.Tariff.Tariff tariff = this.mapper.Map<CreateOrUpdateTariffCommand, Domain.Models.Tariff.Tariff>(command, opts => opts.Items["IsUpdate"] = false);
        tariff.Id = Guid.NewGuid();
        tariff.IsActive = true;
        await this.tariffRepository.AddAsync(tariff, cancellationToken);

        return this.GetMappedHelper<TariffResponse, Domain.Models.Tariff.Tariff>(tariff);
    }
}