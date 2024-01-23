using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

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
        Guid userId = await this.GetTargetUserIdAsync(command.UserId, cancellationToken);
        byte[] imageBytes = await this.ConvertIFormFileToByteArrayAsync(command.File, cancellationToken);
        ImageData data = new ImageData(userId, command.Type, imageBytes);

        if (command.Type == FileTypeEnum.USER_IMAGE)
        {
            ImageData old =
                (await this.imageRepository.FindAsync(i => i.Type == FileTypeEnum.USER_IMAGE && i.UserId == userId,
                    cancellationToken)).FirstOrDefault();

            if (old != null)
            {
                await this.imageRepository.DeleteAsync(old.Id, cancellationToken);
            }
        }
        
        await this.imageRepository.CreateAsync(data, cancellationToken);

        return data.Id;
    }
}