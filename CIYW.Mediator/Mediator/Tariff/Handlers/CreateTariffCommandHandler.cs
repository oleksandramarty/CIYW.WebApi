using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Mediator.Validators.Tariffs;
using MediatR;

namespace CIYW.Mediator.Mediator.Tariff.Handlers;

public class CreateTariffCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateTariffCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository;

    public CreateTariffCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Tariff.Tariff> tariffRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.tariffRepository = tariffRepository;
    }

    public async Task<Guid> Handle(CreateOrUpdateTariffCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateTariffCommand, Guid>(command, () => new CreateOrUpdateTariffCommandValidator(true));
        Domain.Models.Tariff.Tariff tariff = this.mapper.Map<CreateOrUpdateTariffCommand, Domain.Models.Tariff.Tariff>(command, opts => opts.Items["IsUpdate"] = false);
        tariff.Id = Guid.NewGuid();
        tariff.IsActive = true;
        await this.tariffRepository.AddAsync(tariff, cancellationToken);

        return tariff.Id;
    }
}