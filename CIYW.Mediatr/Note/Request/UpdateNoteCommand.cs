using System.ComponentModel.DataAnnotations;
using CIYW.Models.Requests.Common;
using MediatR;

namespace CIYW.Mediatr.Note.Request;

public class UpdateNoteCommand: BaseQuery, IRequest
{
    [StringLength(50)]
    public string Name { get; set; }    
    [StringLength(500)]
    public string Body { get; set; }
}