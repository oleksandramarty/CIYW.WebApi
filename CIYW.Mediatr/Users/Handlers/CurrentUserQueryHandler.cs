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
    private readonly IReadGenericService<User> _userService;
    private readonly IReadGenericService<IdentityUserRole<Guid>> _userRoleService;
    private readonly IReadGenericService<Role> _roleService;


    public CurrentUserQueryHandler(
        IMapper mapper, 
        IReadGenericService<User> userService, 
        IReadGenericService<IdentityUserRole<Guid>> userRoleService, 
        IReadGenericService<Role> roleService)
    {
        _mapper = mapper;
        _userService = userService;
        _userRoleService = userRoleService;
        _roleService = roleService;
    }

    public async Task<CurrentUserResponse> Handle(CurrentUserQuery query, CancellationToken cancellationToken)
    {
        User user = await _userService.GetByIdAsync(query.UserId, cancellationToken);
        if (user == null)
        {
            throw new LoggerException(ErrorMessages.UserNotFound, 404, null, EntityTypeEnum.User.ToString());
        }
        
        IList<IdentityUserRole<Guid>> userRoles =
            await this._userRoleService.GetListByPropertyAsync(ur => ur.UserId == user.Id, cancellationToken);
        if (!userRoles.Any())
        {
            throw new LoggerException(ErrorMessages.RoleNotFound, 404, null, EntityTypeEnum.User.ToString());
        }
        Guid roleId = userRoles.FirstOrDefault().RoleId;
        Role role =
            await this._roleService.GetByPropertyAsync(r => r.Id == roleId, cancellationToken);
        
        CurrentUserResponse response = this._mapper.Map<User, CurrentUserResponse>(user);
        response = this._mapper.Map<Role, CurrentUserResponse>(role, response);

        return response;
    }
}