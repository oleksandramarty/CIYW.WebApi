using CIYW.Const.Enum;
using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediatr.Invoice.Requests;

public class UpdateInvoiceCommand : BaseQuery, IRequest
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public InvoiceTypeEnum Type { get; set; }
}