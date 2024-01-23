using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.Users;
using CIYW.Elasticsearch.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class CreateUserCommandHandler: UserEntityValidatorHelper, IRequestHandler<CreateUserCommand, MappedHelperResponse<UserResponse, User>>
{
    private readonly IMapper mapper;
    private readonly IAuthRepository authRepository;
    private readonly UserManager<User> userManager;
    private readonly IElasticSearchRepository elastic;

    public CreateUserCommandHandler(
        IMapper mapper,
        IAuthRepository authRepository, 
        UserManager<User> userManager,
        IElasticSearchRepository elastic,
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider): base(mapper, entityValidator, currentUserProvider)
    {
        this.mapper = mapper;
        this.authRepository = authRepository;
        this.userManager = userManager;
        this.elastic = elastic;
    }

    public async Task<MappedHelperResponse<UserResponse, User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        await this.CheckUserCommandAsync(null, command, cancellationToken);
        
        User user = this.mapper.Map<CreateUserCommand, User>(command);
        user.TariffId = InitConst.FreeTariffId;
        user.CurrencyId = InitConst.CurrencyUsdId;

        user.UserBalance = new UserBalance
        {
            Id = Guid.NewGuid(),
            CurrencyId = user.CurrencyId,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Amount = 0
        };
      
        var res = await this.userManager.CreateAsync(user, command.Password);

        if (res.Succeeded)
        {
            res = await this.userManager.AddToRoleAsync(user, RoleProvider.User);
        }
        else
        {
            if (res.Errors.Any())
            {
                string creationError = string.Join(Environment.NewLine, res.Errors.Select(e => e.Description));
                throw new LoggerException(creationError, 409);
            }
        }

        List<IdentityUserLogin<Guid>> logins = user.CreateUserLogins();

        await this.authRepository.UpdateUserLoginsAsync(user.Id, logins, cancellationToken);

        this.ValidateExist<User, Guid?>(user, user?.Id);

        UserSearchModel temp = this.mapper.Map<User, UserSearchModel>(user);
        temp.RoleId = InitConst.UserRoleId;    
        await this.elastic.MapEntityAsync<User, UserSearchModel>(user, temp, cancellationToken);

        return this.GetMappedHelper<UserResponse, User>(user);
    }
}
