using AutoMapper;
using CIYW.Domain.Models.Currencies;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Currencies.Requests;
using CIYW.Mediator.Validators.Currencies;
using CIYW.Models.Responses.Currencies;
using MediatR;

namespace CIYW.Mediator.Mediator.Currencies.Handlers;

public class CreateCurrencyCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Currency>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Currency> currencyRepository;

    public CreateCurrencyCommandHandler(
        IMapper mapper,
        IGenericRepository<Currency> currencyRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.currencyRepository = currencyRepository;
    }

    public async Task<MappedHelperResponse<CurrencyResponse, Currency>> Handle(CreateOrUpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        this.ValidateRequest<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Currency>>(command, () => new CreateOrUpdateCurrencyCommandValidator(true));
        Currency currency = this.mapper.Map<CreateOrUpdateCurrencyCommand, Currency>(command, opts => opts.Items["IsUpdate"] = false);
        currency.Id = Guid.NewGuid();
        currency.IsActive = true;
        await this.currencyRepository.AddAsync(currency, cancellationToken);

        return this.GetMappedHelper<CurrencyResponse, Currency>(currency);
    }
}