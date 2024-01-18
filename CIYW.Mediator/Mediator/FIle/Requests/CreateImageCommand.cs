using CIYW.Const.Enums;
using CIYW.Models.Requests.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Mediator.FIle.Requests;

public class CreateImageCommand: IRequest<Guid>
{
    public CreateImageCommand(FileTypeEnum type, IFormFile file)
    {
        Type = type;
        File = file;
    }
    
    public FileTypeEnum Type { get; set; }
    public IFormFile File { get; set; }
}