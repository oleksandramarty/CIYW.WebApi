using AutoMapper;
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

public class UserInvoicesQueryHandler : BasePageableHelper<Domain.Models.Invoice.Invoice>, IRequestHandler<UserInvoicesQuery, ListWithIncludeHelper<InvoiceResponse>>
{
    private ICurrentUserProvider currentUserProvider;
    public UserInvoicesQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository): base(invoiceRepository)
    {
        this.currentUserProvider = currentUserProvider;
    }
    public async Task<ListWithIncludeHelper<InvoiceResponse>> Handle(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        
        return await this.GetPageableResponseAsync<InvoiceResponse>(query.DateRange != null
                ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue &&
                       q.Date >= query.DateRange.DateFrom.Value &&
                       !query.DateRange.DateTo.HasValue ||
                       query.DateRange.DateTo.HasValue && q.Date <= query.DateRange.DateTo.Value &&
                       q.UserId == userId
                : q => q.UserId == userId,
            query,
            cancellationToken,
            q => q.Include(u => u.Category),
            q => q.Include(u => u.Currency),
            q => q.Include(u => u.Note));
    }
}
