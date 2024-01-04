using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Validators.Currencies;
using MediatR;

namespace CIYW.Mediator.Mediator.Currency.Handlers;

public class CreateCurrencyCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCurrencyCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Currency.Currency> currencyRepository;

    public CreateCurrencyCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Currency.Currency> currencyRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.currencyRepository = currencyRepository;
    }

    public async Task<Guid> Handle(CreateOrUpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCurrencyCommand, Guid>(command, () => new CreateOrUpdateCurrencyCommandValidator(true));
        Domain.Models.Currency.Currency currency = this.mapper.Map<CreateOrUpdateCurrencyCommand, Domain.Models.Currency.Currency>(command, opts => opts.Items["IsUpdate"] = false);
        currency.Id = Guid.NewGuid();
        currency.IsActive = true;
        await this.currencyRepository.AddAsync(currency, cancellationToken);

        return currency.Id;
    }
}