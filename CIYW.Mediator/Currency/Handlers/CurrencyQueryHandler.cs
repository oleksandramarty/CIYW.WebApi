﻿using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Currency.Requests;
using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediator.Currency.Handlers;

public class CurrencyQueryHandler: IRequestHandler<CurrencyQuery, CurrencyResponse>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Currency.Currency> currencyRepository;
    private readonly IEntityValidator entityValidator;

    public CurrencyQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Domain.Models.Currency.Currency> currencyRepository, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.currencyRepository = currencyRepository;
        this.entityValidator = entityValidator;
    }

    public async Task<CurrencyResponse> Handle(CurrencyQuery request, CancellationToken cancellationToken)
    {
        Domain.Models.Currency.Currency currency = await this.currencyRepository.GetByIdAsync(request.Id, cancellationToken);

        this.entityValidator.ValidateExist<Domain.Models.Currency.Currency, Guid?>(currency, request.Id);

        CurrencyResponse response = this.mapper.Map<Domain.Models.Currency.Currency, CurrencyResponse>(currency);

        return response;
    }
}