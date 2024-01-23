using CIYW.Const.Enums;
using CIYW.Models.Requests.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class CreateImageCommand: IRequest<Guid>
{
    public CreateImageCommand(FileTypeEnum type, IFormFile file, Guid? userId)
    {
        Type = type;
        File = file;
        UserId = userId;
    }
    
    public FileTypeEnum Type { get; set; }
    public IFormFile File { get; set; }
    public Guid? UserId { get; set; }
}