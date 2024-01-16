﻿using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Invoice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class UserInvoicesQueryHandler(IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository)
    : BasePageableHelper<Domain.Models.Invoice.Invoice>(invoiceRepository),
        IRequestHandler<UserInvoicesQuery, ListWithIncludeHelper<Domain.Models.Invoice.Invoice>>
{
    public async Task<ListWithIncludeHelper<Domain.Models.Invoice.Invoice>> Handle(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        return await this.GetPageableResponseAsync(query.DateRange != null
                ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue &&
                       q.Date >= query.DateRange.DateFrom.Value &&
                       !query.DateRange.DateTo.HasValue ||
                       query.DateRange.DateTo.HasValue && q.Date <= query.DateRange.DateTo.Value
                : null,
            query,
            cancellationToken,
            q => q.Include(u => u.Category),
            q => q.Include(u => u.Currency),
            q => q.Include(u => u.Note));
    }
}
