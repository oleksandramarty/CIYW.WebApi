namespace CIYW.Domain.Models.Invoice;

public class Invoice: BaseWithDateEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    
    public Guid UserId { get; set; }
    public User.User User { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category.Category Category { get; set; }
    
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    
    public DateTime Date { get; set; }
    
    public Guid? NoteId { get; set; }
    public Note Note { get; set; }
}