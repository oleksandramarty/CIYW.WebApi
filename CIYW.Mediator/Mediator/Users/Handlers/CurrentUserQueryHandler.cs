using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Currency.Requests;
using CIYW.Mediator.Mediator.Tariff.Requests;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class CurrentUserQueryHandler: IRequestHandler<CurrentUserQuery, UserResponse>
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;
    private readonly IReadGenericRepository<User> userRepository;
    private readonly IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository;
    private readonly IReadGenericRepository<Role> roleRepository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IEntityValidator entityValidator;


    public CurrentUserQueryHandler(
        IMapper mapper,
        IMediator mediator,
        IReadGenericRepository<User> userRepository, 
        IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository, 
        IReadGenericRepository<Role> roleRepository, 
        ICurrentUserProvider currentUserProvider, 
        IEntityValidator entityValidator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
        this.userRepository = userRepository;
        this.userRoleRepository = userRoleRepository;
        this.roleRepository = roleRepository;
        this.currentUserProvider = currentUserProvider;
        this.entityValidator = entityValidator;
    }

    public async Task<UserResponse> Handle(CurrentUserQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        User user = await this.userRepository.GetWithIncludeAsync(u => u.Id == userId, cancellationToken,
            query => query.Include(u => u.UserBalance));
        this.entityValidator.ValidateExist<User, Guid?>(user, userId);
        
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

        response.Tariff = await this.mediator.Send(new TariffQuery(user.TariffId), cancellationToken);
        response.Currency = await this.mediator.Send(new CurrencyQuery(user.CurrencyId), cancellationToken);

        return response;
    }
}