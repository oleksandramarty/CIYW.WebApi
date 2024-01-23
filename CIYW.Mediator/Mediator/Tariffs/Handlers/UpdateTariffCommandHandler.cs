using AutoMapper;
using CIYW.Domain.Models.Tariffs;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Models.Responses.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariffs.Handlers;

public class UpdateTariffCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Tariff>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Tariff> tariffRepository;

    public UpdateTariffCommandHandler(
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
        this.ValidateRequest<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Tariff>>(command, () => new CreateOrUpdateTariffCommandValidator(false));
        Tariff tariff = await this.tariffRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Tariff, Guid?>(tariff, command.Id);
        
        Tariff updatedTariff = this.mapper.Map<CreateOrUpdateTariffCommand, Tariff>(command, tariff, opts => opts.Items["IsUpdate"] = true);

        await this.tariffRepository.UpdateAsync(updatedTariff, cancellationToken);

        return this.GetMappedHelper<TariffResponse, Tariff>(updatedTariff);
    }
}