using AutoMapper;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class ActiveUserQueryHandler: UserEntityValidatorHelper, IRequestHandler<ActiveUserQuery, MappedHelperResponse<ActiveUserResponse, ActiveUser>>
{
    private readonly IReadGenericRepository<ActiveUser> activeUserRepository;
    
    public ActiveUserQueryHandler(
        IReadGenericRepository<ActiveUser> activeUserRepository,
        IMapper mapper, 
        IEntityValidator entityValidator, 
        ICurrentUserProvider currentUserProvider) : base(mapper, entityValidator, currentUserProvider)
    {
        this.activeUserRepository = activeUserRepository;
    }

    public async Task<MappedHelperResponse<ActiveUserResponse, ActiveUser>> Handle(ActiveUserQuery request, CancellationToken cancellationToken)
    {
        Guid userId = await this.GetUserIdAsync(cancellationToken);
        
        ActiveUser activeUser =
            await this.activeUserRepository.GetByPropertyAsync(a => a.UserId == userId, cancellationToken);
        
        this.ValidateExist<ActiveUser, Guid?>(activeUser, activeUser?.Id);

        return this.GetMappedHelper<ActiveUserResponse, ActiveUser>(activeUser);
    }
}