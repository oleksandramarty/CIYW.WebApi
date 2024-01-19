﻿using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Image;
using CIYW.MongoDB.Models.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Requests;

public class UserImageQuery: BaseNullableQuery, IRequest<MappedHelperResponse<ImageDataResponse, ImageData>>
{
    public UserImageQuery(Guid? id)
    {
        Id = id;
    }
    
    public Guid? UserId { get; set; }
}