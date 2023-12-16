using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Invoice;

public class BalanceInvoicePageableResponse: BasePageableResponse
{
    public IList<BalanceInvoiceResponse> Invoices { get; set; }
}