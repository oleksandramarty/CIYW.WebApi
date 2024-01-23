using AutoMapper;
using CIYW.Domain.Models.Invoices;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Invoices.Requests;
using CIYW.Models.Responses.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Invoices.Handlers;

public class GetInvoiceByIdQueryHandler: UserEntityValidatorHelper, IRequestHandler<GetInvoiceByIdQuery, MappedHelperResponse<InvoiceResponse, Invoice>>
{
    private readonly IMapper mapper;
    private readonly IReadGenericRepository<Invoice> invoiceRepository;

    public GetInvoiceByIdQueryHandler(
        IMapper mapper,
        IReadGenericRepository<Invoice> invoiceRepository,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.invoiceRepository = invoiceRepository;
    }
    
    public async Task<MappedHelperResponse<InvoiceResponse, Invoice>> Handle(GetInvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        Invoice invoice = await this.invoiceRepository.GetWithIncludeAsync(u => u.Id == query.Id, cancellationToken,
            query => query.Include(u => u.Note),
            query => query.Include(u => u.Currency),
            query => query.Include(u => u.Category));
        this.ValidateExist<Invoice, Guid>(invoice, query.Id);
        Guid userId = await this.GetUserIdAsync(cancellationToken);

        await this.HasAccess(invoice, userId, cancellationToken);  
        
        InvoiceResponse mapped =
            this.mapper.Map<Invoice, InvoiceResponse>(invoice);

        return this.GetMappedHelper<InvoiceResponse, Invoice>(invoice);
    }
}