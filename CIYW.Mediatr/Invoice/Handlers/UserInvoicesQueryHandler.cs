using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediatr.Invoice.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Invoice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediatr.Invoice.Handlers;

public class UserInvoicesQueryHandler: IRequestHandler<UserInvoicesQuery, BalanceInvoicePageableResponse>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public UserInvoicesQueryHandler(
        IMapper mapper, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        this.mapper = mapper;
        this.invoiceRepository = invoiceRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<BalanceInvoicePageableResponse> Handle(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<Domain.Models.Invoice.Invoice> invoiceHelper = 
            await this.invoiceRepository.GetListWithIncludeAsync(null,
            query,
            cancellationToken,
            q => q.Include(u => u.Category),
            q => q.Include(u => u.Currency),
            q => q.Include(u => u.Note));

        return new BalanceInvoicePageableResponse
        {
            Invoices = invoiceHelper.Entities.Select(e => this.mapper.Map<Domain.Models.Invoice.Invoice, BalanceInvoiceResponse>(e)).ToList(),
            PageNumber = query.Paginator.PageNumber,
            PageSize = query.Paginator.PageSize,
            TotalCount = invoiceHelper.Total
        };
    }
}