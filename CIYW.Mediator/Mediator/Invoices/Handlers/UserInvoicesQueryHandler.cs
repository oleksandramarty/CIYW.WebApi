using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Invoices.Handlers;

public class UserInvoicesQueryHandler: BasePageableHelper<Invoice>, IRequestHandler<UserInvoicesQuery, ListWithIncludeHelper<InvoiceResponse>>
{
    private ICurrentUserProvider currentUserProvider;
    public UserInvoicesQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IReadGenericRepository<Invoice> invoiceRepository): base(invoiceRepository)
    {
        this.currentUserProvider = currentUserProvider;
    }
    public async Task<ListWithIncludeHelper<InvoiceResponse>> Handle(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        bool isAdmin = await this.currentUserProvider.HasUserInRoleAsync(RoleProvider.Admin, cancellationToken);

        if (query.UserId.HasValue && !isAdmin)
        {
            query.UserId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        }
        
        return await this.GetPageableResponseAsync<InvoiceResponse>(query.DateRange != null
                ? q => 
                    (!query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue && q.Date >= query.DateRange.DateFrom.Value) &&
                       (!query.DateRange.DateTo.HasValue || query.DateRange.DateTo.HasValue && q.Date <= query.DateRange.DateTo.Value) &&
                       (!query.UserId.HasValue || query.UserId.HasValue && q.UserId == query.UserId.Value)
                : q => !query.UserId.HasValue || query.UserId.HasValue && q.UserId == query.UserId.Value,
            query,
            cancellationToken,
            q => q.Include(u => u.Category),
            q => q.Include(u => u.Currency),
            q => q.Include(u => u.Note));
    }
}
