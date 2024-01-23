using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class CurrentUserImageQuery: IRequest<MappedHelperResponse<ImageDataResponse, ImageData>>
{
    
}