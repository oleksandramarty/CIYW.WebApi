using CIYW.Models.Responses.Image;
using CIYW.MongoDB.Models.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Requests;

public class CurrentUserImageQuery: IRequest<MappedHelperResponse<ImageDataResponse, ImageData>>
{
    
}