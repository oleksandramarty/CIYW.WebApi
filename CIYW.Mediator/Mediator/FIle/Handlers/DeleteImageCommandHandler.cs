using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.MongoDB.Models.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Handlers;

public class DeleteImageCommandHandler: BaseFileHelper<ImageData>, IRequestHandler<DeleteImageCommand>
{
    private readonly IMongoDbRepository<ImageData> imageRepository;
    private readonly IMapper mapper;

    public DeleteImageCommandHandler(
        IMongoDbRepository<ImageData> imageRepository,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(imageRepository, mapper, entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
    }
    
    public async Task Handle(DeleteImageCommand command, CancellationToken cancellationToken)
    {
        var imageData = await this.imageRepository.GetByIdAsync(command.Id, cancellationToken);
        
        this.ValidateExist<ImageData, Guid>(imageData, command.Id);
        
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        this.HasAccess(imageData, userId);
        
        await this.imageRepository.DeleteAsync(imageData.Id, cancellationToken);
    }
}