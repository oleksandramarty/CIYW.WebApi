using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediatr.Invoice.Requests;
using MediatR;

namespace CIYW.Mediatr.Invoice.Handlers;

public class CreateInvoiceCommandHandler: IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IGenericRepository<Domain.Models.Invoice.Invoice> _invoiceRepository;
    private readonly ICurrentUserProvider _currentUserProvider;


    public CreateInvoiceCommandHandler(
        IMapper mapper, 
        IMediator mediator, 
        IGenericRepository<Domain.Models.Invoice.Invoice> invoiceRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        _mapper = mapper;
        _mediator = mediator;
        _invoiceRepository = invoiceRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        Guid? noteId = null;

        // TODO check Note
        if (command.NoteCommand != null)
        {
            noteId = await this._mediator.Send(command.NoteCommand, cancellationToken);
        }
        
        Domain.Models.Invoice.Invoice invoice = this._mapper.Map<CreateInvoiceCommand, Domain.Models.Invoice.Invoice>(command);
        invoice.UserId = await this._currentUserProvider.GetUserIdAsync(cancellationToken);
        invoice.NoteId = noteId;

        await this._invoiceRepository.AddAsync(invoice, cancellationToken);

        return invoice.Id;
    }
}