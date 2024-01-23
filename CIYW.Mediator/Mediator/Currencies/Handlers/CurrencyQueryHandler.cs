using AutoMapper;
using CIYW.Domain.Models.Currencies;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Currencies.Requests;
using CIYW.Models.Responses.Currencies;
using MediatR;

namespace CIYW.Mediator.Mediator.Currencies.Handlers;

public class CurrencyQueryHandler: IRequestHandler<CurrencyQuery, MappedHelperResponse<CurrencyResponse, Currency>>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Currency> currencyRepository;
    private readonly IEntityValidator entityValidator;

    public CurrencyQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Currency> currencyRepository, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.currencyRepository = currencyRepository;
        this.entityValidator = entityValidator;
    }

    public async Task<MappedHelperResponse<CurrencyResponse, Currency>> Handle(CurrencyQuery query, CancellationToken cancellationToken)
    {
        Currency currency = await this.currencyRepository.GetByIdAsync(query.Id, cancellationToken);

        this.entityValidator.ValidateExist<Currency, Guid?>(currency, query.Id);

        CurrencyResponse response = this.mapper.Map<Currency, CurrencyResponse>(currency);

        return new MappedHelperResponse<CurrencyResponse, Currency>(response, currency);
    }
}