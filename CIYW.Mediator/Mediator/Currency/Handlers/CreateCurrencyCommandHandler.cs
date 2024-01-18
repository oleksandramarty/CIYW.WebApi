using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Validators.Currencies;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Mediator.Currency.Handlers;

public class CreateCurrencyCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Currency.Currency> currencyRepository;

    public CreateCurrencyCommandHandler(
        IMapper mapper,
        IGenericRepository<Domain.Models.Currency.Currency> currencyRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.currencyRepository = currencyRepository;
    }

    public async Task<MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>> Handle(CreateOrUpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>(command, () => new CreateOrUpdateCurrencyCommandValidator(true));
        Domain.Models.Currency.Currency currency = this.mapper.Map<CreateOrUpdateCurrencyCommand, Domain.Models.Currency.Currency>(command, opts => opts.Items["IsUpdate"] = false);
        currency.Id = Guid.NewGuid();
        currency.IsActive = true;
        await this.currencyRepository.AddAsync(currency, cancellationToken);

        return this.GetMappedHelper<CurrencyResponse, Domain.Models.Currency.Currency>(currency);
    }
}