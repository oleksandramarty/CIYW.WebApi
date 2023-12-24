﻿using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Currency.Requests;
using CIYW.Mediator.Tariff.Requests;
using CIYW.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Users.Handlers;

public class CurrentUserQueryHandler: IRequestHandler<CurrentUserQuery, CurrentUserResponse>
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;
    private readonly IReadGenericRepository<User> userRepository;
    private readonly IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository;
    private readonly IReadGenericRepository<Role> roleRepository;
    private readonly ICurrentUserProvider currentUserProvider;


    public CurrentUserQueryHandler(
        IMapper mapper,
        IMediator mediator,
        IReadGenericRepository<User> userRepository, 
        IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository, 
        IReadGenericRepository<Role> roleRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        this.mapper = mapper;
        this.mediator = mediator;
        this.userRepository = userRepository;
        this.userRoleRepository = userRoleRepository;
        this.roleRepository = roleRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<CurrentUserResponse> Handle(CurrentUserQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
        User user = await this.userRepository.GetWithIncludeAsync(u => u.Id == userId, cancellationToken,
            query => query.Include(u => u.UserBalance));
        
        IList<IdentityUserRole<Guid>> userRoles =
            await this.userRoleRepository.GetListByPropertyAsync(ur => ur.UserId == user.Id, cancellationToken);
        if (!userRoles.Any())
        {
            throw new LoggerException(ErrorMessages.RoleNotFound, 404);
        }
        
        Guid roleId = userRoles.FirstOrDefault().RoleId;
        Role role =
            await this.roleRepository.GetByPropertyAsync(r => r.Id == roleId, cancellationToken);
        
        CurrentUserResponse response = this.mapper.Map<User, CurrentUserResponse>(user);
        response = this.mapper.Map<Role, CurrentUserResponse>(role, response);
        response = this.mapper.Map<UserBalance, CurrentUserResponse>(user.UserBalance, response);

        response.Tariff = await this.mediator.Send(new TariffQuery(user.TariffId), cancellationToken);
        response.Currency = await this.mediator.Send(new CurrencyQuery(user.CurrencyId), cancellationToken);

        return response;
    }
}