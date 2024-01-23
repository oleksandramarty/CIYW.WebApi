using System.ComponentModel.DataAnnotations;
using CIYW.Domain.Models.Invoices;

namespace CIYW.Domain.Models.Notes;

public class Note: BaseWithDateEntity
{    
    [Required]
    [StringLength(50)]
    public string Name { get; set; }    
    [Required]
    [StringLength(500)]
    public string Body { get; set; }
    
    public Guid? InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    
    public Guid UserId { get; set; }
    public Users.User User { get; set; }
}