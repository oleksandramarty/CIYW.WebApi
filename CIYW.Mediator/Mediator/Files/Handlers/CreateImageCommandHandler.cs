using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.Models.Responses.Images;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

public class CreateImageCommandHandler: BaseFileHelper<ImageData>, IRequestHandler<CreateImageCommand, MappedHelperResponse<ImageDataResponse, ImageData>>
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

    public async Task<MappedHelperResponse<ImageDataResponse, ImageData>> Handle(CreateImageCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetTargetUserIdAsync(command.UserId, cancellationToken);
        byte[] imageBytes = await this.ConvertIFormFileToByteArrayAsync(command.File, cancellationToken);
        ImageData data = new ImageData(userId, command.Type, command.File.Name, imageBytes);

        if (command.Type == FileTypeEnum.USER_IMAGE)
        {
            ImageData old =
                (await this.imageRepository.FindAsync(i => i.Type == FileTypeEnum.USER_IMAGE && i.EntityId == userId,
                    cancellationToken)).FirstOrDefault();

            if (old != null)
            {
                await this.imageRepository.DeleteAsync(old.Id, cancellationToken);
            }
        }
        
        await this.imageRepository.CreateAsync(data, cancellationToken);

        ImageData result = (await this.imageRepository.FindAsync(i => i.Id == data.Id, cancellationToken)).FirstOrDefault();
        
        this.ValidateExist(result, data.Id);
        
        return this.GetMappedHelper<ImageDataResponse, ImageData>(result);
    }
}