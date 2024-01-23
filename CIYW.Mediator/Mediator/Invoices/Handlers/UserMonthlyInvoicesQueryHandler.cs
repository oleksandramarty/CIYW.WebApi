using AutoMapper;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoices;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoices.Handlers;

public class UserMonthlyInvoicesQueryHandler: IRequestHandler<UserMonthlyInvoicesQuery, ListWithIncludeHelper<InvoiceResponse>>
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public UserMonthlyInvoicesQueryHandler(
        IMapper mapper, 
        IMediator mediator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
    }

    public async Task<ListWithIncludeHelper<InvoiceResponse>> Handle(UserMonthlyInvoicesQuery query, CancellationToken cancellationToken)
    {
        UserInvoicesQuery monthQuery = new UserInvoicesQuery(FilterExtension.CreateDefaultFilter("Date", query.Paginator))
            {
                UserId = query.UserId
            };
        
        ListWithIncludeHelper<InvoiceResponse> response = await this.mediator.Send(monthQuery, cancellationToken);
        return response;
    }
}