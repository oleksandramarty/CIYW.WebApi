using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class UserImageQuery: BaseNullableQuery, IRequest<MappedHelperResponse<ImageDataResponse, ImageData>>
{
    public UserImageQuery(Guid? id)
    {
        Id = id;
    }
    
    public Guid? UserId { get; set; }
}