using AutoMapper;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Invoice;
using MediatR;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class UserMonthlyInvoicesQueryHandler: IRequestHandler<UserMonthlyInvoicesQuery, ListWithIncludeHelper<Domain.Models.Invoice.Invoice>>
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

    public async Task<ListWithIncludeHelper<Domain.Models.Invoice.Invoice>> Handle(UserMonthlyInvoicesQuery query, CancellationToken cancellationToken)
    {
        UserInvoicesQuery invoicesQuery =
            this.mapper.Map<BaseFilterQuery, UserInvoicesQuery>(FilterExtension.CreateDefaultFilter("Date"));
        ListWithIncludeHelper<Domain.Models.Invoice.Invoice> response = await this.mediator.Send(invoicesQuery, cancellationToken);
        return response;
    }
}