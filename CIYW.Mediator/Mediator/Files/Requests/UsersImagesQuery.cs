using CIYW.Const.Enums;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Requests;

public class UsersImagesQuery: BaseFileFilterQuery, IRequest<ListWithIncludeHelper<ImageDataResponse>>
{
}