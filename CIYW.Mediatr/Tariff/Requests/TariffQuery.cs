﻿using CIYW.Models.Responses.Tariff;
using MediatR;

namespace CIYW.Mediatr.Tariff.Requests;

public class TariffQuery: IRequest<TariffResponse>
{
    public TariffQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}