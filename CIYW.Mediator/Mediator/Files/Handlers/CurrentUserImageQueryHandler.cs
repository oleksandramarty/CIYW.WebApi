using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

public class CurrentUserImageQueryHandler: UserEntityValidatorHelper, IRequestHandler<CurrentUserImageQuery, MappedHelperResponse<ImageDataResponse, ImageData>>
{    
    private readonly IMongoDbRepository<ImageData> imageRepository;
    private readonly IMapper mapper;
    
    public CurrentUserImageQueryHandler(
        IMongoDbRepository<ImageData> imageRepository, 
        IMapper mapper,
        IEntityValidator entityValidator,
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
        this.mapper = mapper;
    }
    
    public async Task<MappedHelperResponse<ImageDataResponse, ImageData>> Handle(CurrentUserImageQuery request, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        IEnumerable<ImageData> temp = await this.imageRepository.FindAsync(i => i.UserId == userId && i.Type == FileTypeEnum.USER_IMAGE,
            cancellationToken);

        if (!temp.Any())
        {
            return null;
        }

        ImageData imageData = temp.First();
        
        this.ValidateExist<ImageData, Guid>(imageData, imageData.Id);

        return this.GetMappedHelper<ImageDataResponse, ImageData>(imageData);
    }
}