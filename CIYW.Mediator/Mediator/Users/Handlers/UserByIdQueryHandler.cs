using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Currencies.Requests;
using CIYW.Mediator.Mediator.Tariffs.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UserByIdQueryHandler: UserEntityValidatorHelper, IRequestHandler<UserByIdQuery, MappedHelperResponse<UserResponse, User>>
{
    private ICurrentUserProvider currentUserProvider;
    private IReadGenericRepository<User> userRepository;
    private readonly IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository;
    private readonly IReadGenericRepository<Role> roleRepository;
    private IMapper mapper;
    private IMediator mediator;

    public UserByIdQueryHandler(
        IMediator mediator,
        IMapper mapper,
        IReadGenericRepository<User> userRepository,
        IReadGenericRepository<Role> roleRepository, 
        IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository,
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator
        ): base(mapper, entityValidator, currentUserProvider)
    {
        this.currentUserProvider = currentUserProvider;
        this.roleRepository = roleRepository;
        this.userRoleRepository = userRoleRepository;
        this.mediator = mediator;
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<MappedHelperResponse<UserResponse, User>> Handle(UserByIdQuery query, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);

        User user = await this.userRepository.GetWithIncludeAsync(u => u.Id == query.Id, cancellationToken,
            query => query.Include(u => u.UserBalance));
        this.ValidateExist<User, Guid?>(user, query.Id);
        
        IList<IdentityUserRole<Guid>> userRoles =
            await this.userRoleRepository.GetListByPropertyAsync(ur => ur.UserId == user.Id, cancellationToken);
        if (!userRoles.Any())
        {
            throw new LoggerException(ErrorMessages.RoleNotFound, 404);
        }
        
        Guid roleId = userRoles.FirstOrDefault().RoleId;
        Role role =
            await this.roleRepository.GetByPropertyAsync(r => r.Id == roleId, cancellationToken);
        
        UserResponse response = this.mapper.Map<User, UserResponse>(user);
        response.RoleId = role.Id;
        response.Role = role.Name;
        response.UserBalance = this.mapper.Map<UserBalance, UserBalanceResponse>(user.UserBalance);
        response.BalanceAmount = user.UserBalance.Amount;

        response.Tariff = (await this.mediator.Send(new TariffQuery(user.TariffId), cancellationToken)).MappedEntity;
        response.Currency = (await this.mediator.Send(new CurrencyQuery(user.CurrencyId), cancellationToken)).MappedEntity;

        return new MappedHelperResponse<UserResponse, User>(response, user);
    }
}