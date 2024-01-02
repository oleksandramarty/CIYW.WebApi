using AutoMapper;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediatr.Invoice.Requests;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediatr.Invoice.Handlers;

public class UserMonthlyInvoicesQueryHandler: IRequestHandler<UserMonthlyInvoicesQuery, BalanceInvoicePageableResponse>
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

    public async Task<BalanceInvoicePageableResponse> Handle(UserMonthlyInvoicesQuery query, CancellationToken cancellationToken)
    {
        UserInvoicesQuery invoicesQuery =
            this.mapper.Map<BaseFilterQuery, UserInvoicesQuery>(FilterExtension.CreateDefaultFilter("Date"));
        BalanceInvoicePageableResponse response = await this.mediator.Send(invoicesQuery, cancellationToken);
        return response;
    }
}