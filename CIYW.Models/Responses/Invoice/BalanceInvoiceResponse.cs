using CIYW.Const.Enum;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Category;
using CIYW.Models.Responses.Currency;
using CIYW.Models.Responses.Note;

namespace CIYW.Models.Responses.Invoice;

public class BalanceInvoiceResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    
    public CategoryResponse Category { get; set; }
    public CurrencyResponse Currency { get; set; }
    
    public DateTime Date { get; set; }
    
    public NoteResponse Note { get; set; }
    
    public InvoiceTypeEnum Type { get; set; }
}