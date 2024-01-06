using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currency;

namespace CIYW.Models.Responses.Invoice;

public class BalanceInvoicePageableResponse: Paginator
{
    public IList<BalanceInvoiceResponse> Invoices { get; set; }
    
    public decimal Balance { get; set; }
    
    public CurrencyResponse Currency { get; set; }
    
    public int TotalCount { get; set; }
}