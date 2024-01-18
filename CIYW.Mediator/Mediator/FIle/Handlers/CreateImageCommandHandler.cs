using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.FIle.Requests;
using CIYW.MongoDB.Models.Image;
using MediatR;

namespace CIYW.Mediator.Mediator.FIle.Handlers;

public class CreateImageCommandHandler: BaseFileHelper<ImageData>, IRequestHandler<CreateImageCommand, Guid>
{
    private readonly IMongoDbRepository<ImageData> imageRepository;
    private readonly IMapper mapper;

    public CreateImageCommandHandler(
        IMongoDbRepository<ImageData> imageRepository,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(imageRepository, mapper, entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
    }

    public async Task<Guid> Handle(CreateImageCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        byte[] imageBytes = await this.ConvertIFormFileToByteArrayAsync(command.File, cancellationToken);
        ImageData data = new ImageData(userId, command.Type, imageBytes);
        await this.imageRepository.CreateAsync(data, cancellationToken);

        return data.Id;
    }
}