using System.ComponentModel.DataAnnotations;
using CIYW.Domain.Models.User;
using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Note;

public class NoteResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; } 
    public string Body { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid UserId { get; set; }
}