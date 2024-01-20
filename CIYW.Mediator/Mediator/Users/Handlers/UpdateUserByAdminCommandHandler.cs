using AutoMapper;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Elasticsearch.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UpdateUserByAdminCommandHandler: UserEntityValidatorHelper, IRequestHandler<UpdateUserByAdminCommand, MappedHelperResponse<UserResponse, User>>
{
    private readonly IMapper mapper;
    private readonly IAuthRepository authRepository;
    private readonly UserManager<User> userManager;
    private readonly IGenericRepository<User> userRepository;
    private readonly IElasticSearchRepository elastic;

    public UpdateUserByAdminCommandHandler(
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
    
    public async Task<MappedHelperResponse<UserResponse, User>> Handle(UpdateUserByAdminCommand command, CancellationToken cancellationToken)
    {
        await this.CheckUserCommandAsync(command.Id, command, cancellationToken);

        User user = await this.userRepository.GetByIdAsync(command.Id, cancellationToken);
        
        this.ValidateExist<User, Guid?>(user, command.Id);
        
        user = this.mapper.Map<UpdateUserByAdminCommand, User>(command, user);

        await this.authRepository.UpdateUserLoginsAsync(user, cancellationToken);
        
        await this.elastic.MapEntityAsync<User, UserSearchModel>(user, cancellationToken);

        UserResponse mapped = this.mapper.Map<User, UserResponse>(user);
        mapped.Role = RoleProvider.User;
        mapped.RoleId = InitConst.UserRoleId;

        return new MappedHelperResponse<UserResponse, User>(mapped, user);
    }
}