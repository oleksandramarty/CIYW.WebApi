using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class UserByIdQueryHandler: UserEntityValidatorHelper, IRequestHandler<UserByIdQuery, MappedHelperResponse<UserResponse, User>>
{
    private IReadGenericRepository<User> userRepository;
    private IMapper mapper;

    public UserByIdQueryHandler(
        IReadGenericRepository<User> userRepository,
        IMapper mapper,
        ICurrentUserProvider currentUser, 
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUser)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<MappedHelperResponse<UserResponse, User>> Handle(UserByIdQuery query, CancellationToken cancellationToken)
    {
        await this.IsUserAdminAsync(cancellationToken);

        User user = await this.userRepository.GetWithIncludeAsync(
            u => u.Id == query.Id,
            cancellationToken,
            q =>
                q.Include(t => t.UserBalance)
                    .ThenInclude(t => t.Currency));

        return this.GetMappedHelper<UserResponse, User>(user);
    }
}