using CIYW.Const.Enums;
using CIYW.Models.Requests.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class UpdateImageCommand: BaseQuery, IRequest
{
    public UpdateImageCommand(Guid id, IFormFile file)
    {
        Id = id;
        File = file;
    }
    
    public IFormFile File { get; set; }
}