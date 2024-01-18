using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UserByIdQueryHandler: UserEntityValidatorHelper, IRequestHandler<UserByIdQuery, UserResponse>
{
    private IReadGenericRepository<User> userRepository;
    private IMapper mapper;

    public UserByIdQueryHandler(
        IReadGenericRepository<User> userRepository,
        IMapper mapper,
        ICurrentUserProvider currentUser, 
        IEntityValidator entityValidator): base(entityValidator, currentUser)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<UserResponse> Handle(UserByIdQuery query, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);

        User user = await this.userRepository.GetWithIncludeAsync(
            u => u.Id == query.Id,
            cancellationToken,
            q =>
                q.Include(t => t.UserBalance)
                    .ThenInclude(t => t.Currency));

        UserResponse response = this.mapper.Map<User, UserResponse>(user);

        return response;
    }
}