using AutoMapper;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoice.Requests;
using CIYW.Models.Responses.Invoice;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Invoice.Handlers;

public class GetInvoiceByIdQueryHandler: UserEntityValidatorHelper, IRequestHandler<GetInvoiceByIdQuery, MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository;

    public GetInvoiceByIdQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.invoiceRepository = invoiceRepository;
    }
    
    public async Task<MappedHelperResponse<InvoiceResponse, Domain.Models.Invoice.Invoice>> Handle(GetInvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Models.Invoice.Invoice invoice = await this.invoiceRepository.GetWithIncludeAsync(u => u.Id == query.Id, cancellationToken,
            query => query.Include(u => u.Note),
            query => query.Include(u => u.Currency),
            query => query.Include(u => u.Category));
        this.ValidateExist<Domain.Models.Invoice.Invoice, Guid>(invoice, query.Id);
        Guid userId = await this.GetUserIdAsync(cancellationToken);

        bool isAdmin = await this.HasUserAdminAsync(cancellationToken);

        if (!isAdmin)
        {
            this.HasAccess(invoice, userId);          
        }
        
        InvoiceResponse mapped =
            this.mapper.Map<Domain.Models.Invoice.Invoice, InvoiceResponse>(invoice);

        return this.GetMappedHelper<InvoiceResponse, Domain.Models.Invoice.Invoice>(invoice);
    }
}