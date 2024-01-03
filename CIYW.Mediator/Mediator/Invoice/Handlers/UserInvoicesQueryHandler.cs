using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Invoice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class UserInvoicesQueryHandler: IRequestHandler<UserInvoicesQuery, BalanceInvoicePageableResponse>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;
    private readonly IReadGenericRepository<Domain.Models.User.User> userRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;

    public UserInvoicesQueryHandler(
        IMapper mapper, 
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository,
        IReadGenericRepository<Domain.Models.User.User> userRepository,
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.invoiceRepository = invoiceRepository;
        this.userRepository = userRepository;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    public async Task<BalanceInvoicePageableResponse> Handle(UserInvoicesQuery query, CancellationToken cancellationToken)
    {
        ListWithIncludeHelper<Domain.Models.Invoice.Invoice> invoiceHelper = 
            await this.invoiceRepository.GetListWithIncludeAsync(
                query.DateRange != null 
                    ? q => !query.DateRange.DateFrom.HasValue || query.DateRange.DateFrom.HasValue && q.Date >= query.DateRange.DateFrom.Value &&
                        !query.DateRange.DateTo.HasValue || query.DateRange.DateTo.HasValue && q.Date <= query.DateRange.DateTo.Value
                    : null,
            query,
            cancellationToken,
            q => q.Include(u => u.Category),
            q => q.Include(u => u.Currency),
            q => q.Include(u => u.Note));

        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        User user = await this.userRepository.GetWithIncludeAsync(u => u.Id == userId, cancellationToken,
            query => query.Include(u => u.UserBalance),
            query => query.Include(u => u.Currency));
        
        this.entityValidator.ValidateExist<User, Guid?>(user, userId);

        return new BalanceInvoicePageableResponse
        {
            Invoices = invoiceHelper.Entities.Select(e => this.mapper.Map<Domain.Models.Invoice.Invoice, BalanceInvoiceResponse>(e)).ToList(),
            PageNumber = query.Paginator.PageNumber,
            PageSize = query.Paginator.PageSize,
            TotalCount = invoiceHelper.Total,
            Balance = user.UserBalance.Amount,
            Currency = this.mapper.Map<Domain.Models.Currency.Currency, CurrencyResponse>(user.Currency)
        };
    }
}