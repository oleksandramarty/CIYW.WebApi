using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Invoice;

public class BalanceInvoicePageableResponse: IBasePageableResponse
{
    public IList<BalanceInvoiceResponse> Invoices { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}