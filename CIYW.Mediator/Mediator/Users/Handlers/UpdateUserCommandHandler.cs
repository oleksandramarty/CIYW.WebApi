using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UpdateUserCommandHandler: UserEntityValidatorHelper, IRequestHandler<UpdateUserCommand, MappedHelperResponse<UserResponse, User>>
{
    private readonly IMapper mapper;
    private readonly IAuthRepository authRepository;
    private readonly UserManager<User> userManager;
    private readonly IGenericRepository<User> userRepository;
    private readonly IElasticSearchRepository elastic;

    public UpdateUserCommandHandler(
        IMapper mapper,
        IAuthRepository authRepository, 
        UserManager<User> userManager,
        IGenericRepository<User> userRepository,
        IElasticSearchRepository elastic,
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.authRepository = authRepository;
        this.userManager = userManager;
        this.userRepository = userRepository;
        this.elastic = elastic;
    }
    
    public async Task<MappedHelperResponse<UserResponse, User>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        await this.CheckUserCommandAsync(userId, command, cancellationToken);

        User user = await this.userRepository.GetByIdAsync(userId, cancellationToken);
        
        this.ValidateExist<User, Guid?>(user, userId);
        
        user = this.mapper.Map<UpdateUserCommand, User>(command, user);

        await this.authRepository.UpdateUserLoginsAsync(user, cancellationToken);
        
        await this.elastic.MapEntityAsync<User, UserSearchModel>(user, cancellationToken);

        return this.GetMappedHelper<UserResponse, User>(user);
    }
}