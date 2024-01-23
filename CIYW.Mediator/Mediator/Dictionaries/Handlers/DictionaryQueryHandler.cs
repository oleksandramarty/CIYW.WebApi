﻿using CIYW.Const.Enums;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Handlers;

public class DictionaryQueryHandler: IRequestHandler<DictionaryQuery, DictionariesResponse>
{
    private readonly IMediator mediator;

    public DictionaryQueryHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<DictionariesResponse> Handle(DictionaryQuery request, CancellationToken cancellationToken)
    {
        DictionariesResponse response = new DictionariesResponse();

        response.Currencies = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.CURRENCY), cancellationToken);
        response.Categories = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.CATEGORY), cancellationToken);
        response.Roles = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.ROLE), cancellationToken);
        response.Tariffs = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.TARIFF), cancellationToken);
        response.InvoiceTypes = await this.mediator.Send(new DictionaryEnumQuery(EntityTypeEnum.INVOICE_TYPE), cancellationToken);

        return response;
    }
}