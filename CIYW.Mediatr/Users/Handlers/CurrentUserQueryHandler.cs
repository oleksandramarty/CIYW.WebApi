using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediatr.Base.Requests;
using CIYW.Mediatr.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediatr.Users.Handlers;

public class CurrentUserQueryHandler: IRequestHandler<CurrentUserQuery, CurrentUserResponse>
{
    private readonly IMapper _mapper;
    private readonly IReadGenericRepository<User> _userRepository;
    private readonly IReadGenericRepository<IdentityUserRole<Guid>> _userRoleRepository;
    private readonly IReadGenericRepository<Role> _roleRepository;
    private readonly ICurrentUserProvider _currentUserProvider;


    public CurrentUserQueryHandler(
        IMapper mapper, 
        IReadGenericRepository<User> userRepository, 
        IReadGenericRepository<IdentityUserRole<Guid>> userRoleRepository, 
        IReadGenericRepository<Role> roleRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<CurrentUserResponse> Handle(CurrentUserQuery query, CancellationToken cancellationToken)
    {
        Guid userId = await this._currentUserProvider.GetUserIdAsync(cancellationToken);
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new LoggerException(ErrorMessages.UserNotFound, 404, null, EntityTypeEnum.User.ToString());
        }
        
        IList<IdentityUserRole<Guid>> userRoles =
            await this._userRoleRepository.GetListByPropertyAsync(ur => ur.UserId == user.Id, cancellationToken);
        if (!userRoles.Any())
        {
            throw new LoggerException(ErrorMessages.RoleNotFound, 404, null, EntityTypeEnum.User.ToString());
        }
        Guid roleId = userRoles.FirstOrDefault().RoleId;
        Role role =
            await this._roleRepository.GetByPropertyAsync(r => r.Id == roleId, cancellationToken);
        
        CurrentUserResponse response = this._mapper.Map<User, CurrentUserResponse>(user);
        response = this._mapper.Map<Role, CurrentUserResponse>(role, response);

        return response;
    }
}