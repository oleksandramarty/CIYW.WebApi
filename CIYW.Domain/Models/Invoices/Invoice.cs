using CIYW.Const.Enums;
using CIYW.Domain.Models.Categories;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Notes;

namespace CIYW.Domain.Models.Invoices;

public class Invoice: BaseWithDateEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    
    public Guid UserId { get; set; }
    public Users.User User { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    
    public DateTime? Mapped { get; set; }
    
    public DateTime Date { get; set; }
    
    public Guid? NoteId { get; set; }
    public Note Note { get; set; }
    
    public InvoiceTypeEnum Type { get; set; }
}