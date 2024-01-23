using CIYW.Const.Enums;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class CreateImageCommand: IRequest<MappedHelperResponse<ImageDataResponse, ImageData>>
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