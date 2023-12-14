using System.ComponentModel.DataAnnotations;

namespace CIYW.Domain.Models;

public class Note: BaseWithDateEntity
{    
    [Required]
    [StringLength(50)]
    public string Name { get; set; }    
    [Required]
    [StringLength(500)]
    public string Body { get; set; }
    
    public Guid? InvoiceId { get; set; }
    public Invoice.Invoice Invoice { get; set; }
    
    public Guid UserId { get; set; }
    public User.User User { get; set; }
}