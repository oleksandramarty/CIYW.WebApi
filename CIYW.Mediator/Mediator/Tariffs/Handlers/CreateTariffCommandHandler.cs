using AutoMapper;
using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Handlers;

public class CreateTariffCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Tariff>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Tariff> tariffRepository;

    public CreateTariffCommandHandler(
        IMapper mapper,
        IGenericRepository<Tariff> tariffRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
    }

    public async Task<MappedHelperResponse<TariffResponse, Tariff>> Handle(CreateOrUpdateTariffCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Tariff>>(command, () => new CreateOrUpdateTariffCommandValidator(true));
        Tariff tariff = this.mapper.Map<CreateOrUpdateTariffCommand, Tariff>(command, opts => opts.Items["IsUpdate"] = false);
        tariff.Id = Guid.NewGuid();
        tariff.IsActive = true;
        await this.tariffRepository.AddAsync(tariff, cancellationToken);

        return this.GetMappedHelper<TariffResponse, Tariff>(tariff);
    }
}