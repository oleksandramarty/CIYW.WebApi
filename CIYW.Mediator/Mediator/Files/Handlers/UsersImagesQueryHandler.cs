using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

public class UsersImagesQueryHandler: BaseFileHelper<ImageData>, IRequestHandler<UsersImagesQuery, ListWithIncludeHelper<ImageDataResponse>>
{
    private readonly IMongoDbRepository<ImageData> imageRepository;
    private readonly IMapper mapper;

    public UsersImagesQueryHandler(
        IMongoDbRepository<ImageData> imageRepository, 
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(imageRepository, mapper, entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
        this.mapper = mapper;
    }

    public async Task<ListWithIncludeHelper<ImageDataResponse>> Handle(UsersImagesQuery query, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);
        
        IList<Guid> userIds = query.Ids.Ids;

        query.Ids.Ids = (await this.imageRepository.FindAsync(i => userIds.Any(x => x == i.EntityId), cancellationToken))
            .Select(x => x.Id).ToList();
        
        return await this.GetPageableResponseAsync<ImageDataResponse>(query, cancellationToken);
    }
}