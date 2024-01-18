using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Requests;

public class UsersImagesQuery: BaseFilterQuery, IRequest<ListWithIncludeHelper<ImageDataResponse>>
{
}