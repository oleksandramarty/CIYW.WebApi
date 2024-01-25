using AutoMapper;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Mediator.Common;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class CheckTemporaryPasswordQueryHandler: UserEntityValidatorHelper, IRequestHandler<CheckTemporaryPasswordQuery, bool>
{
    private readonly IGenericRepository<User> userRepository;
    
    public CheckTemporaryPasswordQueryHandler(
        IGenericRepository<User> userRepository,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.userRepository = userRepository;
    }
    public async Task<bool> Handle(CheckTemporaryPasswordQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);

        User user = await this.userRepository.GetByIdAsync(userId, cancellationToken);
        
        this.ValidateExist<User, Guid>(user, userId);

        return user.IsTemporaryPassword;
    }
}