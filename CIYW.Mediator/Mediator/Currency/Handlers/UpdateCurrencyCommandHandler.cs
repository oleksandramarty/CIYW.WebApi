using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Validators.Currencies;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Mediator.Currency.Handlers;

public class UpdateCurrencyCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>
{
    private readonly IMapper mapper;
    private readonly IGenericRepository<Domain.Models.Currency.Currency> currencyRepository;

    public UpdateCurrencyCommandHandler(
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
        this.ValidateRequest<CreateOrUpdateCurrencyCommand, MappedHelperResponse<CurrencyResponse, Domain.Models.Currency.Currency>>(command, () => new CreateOrUpdateCurrencyCommandValidator(false));
        Domain.Models.Currency.Currency currency = await this.currencyRepository.GetByIdAsync(command.Id.Value, cancellationToken);

        this.ValidateExist<Domain.Models.Currency.Currency, Guid?>(currency, command.Id);
        
        Domain.Models.Currency.Currency updatedCurrency = this.mapper.Map<CreateOrUpdateCurrencyCommand, Domain.Models.Currency.Currency>(command, currency, opts => opts.Items["IsUpdate"] = true);

        await this.currencyRepository.UpdateAsync(updatedCurrency, cancellationToken);

        return this.GetMappedHelper<CurrencyResponse, Domain.Models.Currency.Currency>(updatedCurrency);
    }
}