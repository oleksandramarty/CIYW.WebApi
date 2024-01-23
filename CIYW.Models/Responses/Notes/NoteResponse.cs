using System.ComponentModel.DataAnnotations;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Currencies;
using CIYW.Models.Responses.Invoices;
using CIYW.Models.Responses.Users;

namespace CIYW.Models.Responses.Notes;

public class NoteResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; } 
    public string Body { get; set; }
    
    public Guid UserId { get; set; }
    public UserResponse User { get; set; }
    public Guid? InvoiceId { get; set; }
    public InvoiceResponse Invoice { get; set; }
}