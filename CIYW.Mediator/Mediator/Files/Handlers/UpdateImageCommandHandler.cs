﻿using AutoMapper;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Files.Requests;
using CIYW.MongoDB.Models.Images;
using MediatR;

namespace CIYW.Mediator.Mediator.Files.Handlers;

public class UpdateImageCommandHandler: BaseFileHelper<ImageData>, IRequestHandler<UpdateImageCommand>
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

    public async Task Handle(UpdateImageCommand command, CancellationToken cancellationToken)
    {
        var imageData = await this.imageRepository.GetByIdAsync(command.Id, cancellationToken);
        
        this.ValidateExist<ImageData, Guid>(imageData, command.Id);
        
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        await this.HasAccess(imageData, userId, cancellationToken);
    
        imageData.Data = await this.ConvertIFormFileToByteArrayAsync(command.File, cancellationToken);
        
        await this.imageRepository.UpdateAsync(imageData.Id, imageData);
    }
}