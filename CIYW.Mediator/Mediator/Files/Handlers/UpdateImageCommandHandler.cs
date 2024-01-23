using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

public class UpdateImageCommandHandler: BaseFileHelper<ImageData>, IRequestHandler<UpdateImageCommand, MappedHelperResponse<ImageDataResponse, ImageData>>
{
    private readonly IMongoDbRepository<ImageData> imageRepository;
    private readonly IMapper mapper;

    public UpdateImageCommandHandler(
        IMongoDbRepository<ImageData> imageRepository,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(imageRepository, mapper, entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
    }

    public async Task<MappedHelperResponse<ImageDataResponse, ImageData>> Handle(UpdateImageCommand command, CancellationToken cancellationToken)
    {
        var imageData = await this.imageRepository.GetByIdAsync(command.Id, cancellationToken);
        
        this.ValidateExist<ImageData, Guid>(imageData, command.Id);
        
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        await this.HasAccess(imageData, userId, cancellationToken, nameof(ImageData.EntityId));
    
        imageData.Data = await this.ConvertIFormFileToByteArrayAsync(command.File, cancellationToken);
        imageData.Name = command.File.Name;
        
        await this.imageRepository.UpdateAsync(imageData.Id, imageData);

        return this.GetMappedHelper<ImageDataResponse, ImageData>(imageData);
    }
}