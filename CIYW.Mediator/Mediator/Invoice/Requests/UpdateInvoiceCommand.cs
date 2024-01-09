﻿using CIYW.Const.Enum;
using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Requests;

public class UpdateInvoiceCommand : BaseNullableQuery, IRequest<Domain.Models.Invoice.Invoice>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public InvoiceTypeEnum Type { get; set; }
}