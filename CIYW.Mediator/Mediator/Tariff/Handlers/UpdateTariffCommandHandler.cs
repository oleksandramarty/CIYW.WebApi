using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Mediator.Validators.Tariffs;
using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Handlers;

public class UpdateTariffCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;

    public UpdateTariffCommandHandler(
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
        this.ValidateRequest<CreateOrUpdateTariffCommand, MappedHelperResponse<TariffResponse, Domain.Models.Tariff.Tariff>>(command, () => new CreateOrUpdateTariffCommandValidator(false));
        Domain.Models.Tariff.Tariff tariff = await this.tariffRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Domain.Models.Tariff.Tariff, Guid?>(tariff, command.Id);
        
        Domain.Models.Tariff.Tariff updatedTariff = this.mapper.Map<CreateOrUpdateTariffCommand, Domain.Models.Tariff.Tariff>(command, tariff, opts => opts.Items["IsUpdate"] = true);

        await this.tariffRepository.UpdateAsync(updatedTariff, cancellationToken);

        return this.GetMappedHelper<TariffResponse, Domain.Models.Tariff.Tariff>(updatedTariff);
    }
}