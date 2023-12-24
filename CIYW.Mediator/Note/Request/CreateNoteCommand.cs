using System.ComponentModel.DataAnnotations;
using MediatR;

namespace CIYW.Mediator.Note.Request;

public class CreateNoteCommand: IRequest<Guid>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }    
    [Required]
    [StringLength(500)]
    public string Body { get; set; }
}