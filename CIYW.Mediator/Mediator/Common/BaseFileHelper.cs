using AutoMapper;
using CIYW.Interfaces;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.MongoDB.Models.Image;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Mediator.Common;

public class BaseFileHelper<T>: UserEntityValidatorHelper
{
    private readonly IMongoDbRepository<T> imageRepository;
    private readonly IMapper mapper;
    public BaseFileHelper(
        IMongoDbRepository<T> imageRepository, 
        IMapper mapper,
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider) : base(entityValidator, currentUserProvider)
    {
        this.imageRepository = imageRepository;
        this.mapper = mapper;
    }
    
    protected async Task<byte[]> ConvertIFormFileToByteArrayAsync(IFormFile avatarFile,
        CancellationToken cancellationToken)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await avatarFile.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }
    }
    
    protected async Task<ListWithIncludeHelper<TResponse>> GetPageableResponseAsync<TResponse>(
        BaseFilterQuery filter,
        CancellationToken cancellationToken)
    {
        return await this.imageRepository.GetPageableListAsync<TResponse>(filter, cancellationToken);
    }
}