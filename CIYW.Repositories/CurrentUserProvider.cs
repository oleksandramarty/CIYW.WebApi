using System.Security.Claims;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Repositories;

public class CurrentUserProvider: ICurrentUserProvider
{
    private readonly UserManager<User> userManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly DataContext context;

    public CurrentUserProvider(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        this.userManager = userManager;
        this.httpContextAccessor = httpContextAccessor;
        this.context = context;
    }
    
    public async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken)
    {
        // Try to get user ID from HttpContext
        var userId = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
            return Guid.Parse(userId);
        }

        // If HttpContext is not available or user ID is not found, fall back to UserManager
        var user = await this.userManager.GetUserAsync(this.httpContextAccessor.HttpContext?.User);

        if (user == null)
        {
            throw new LoggerException(ErrorMessages.UserNotFound, 404);
        }
        
        return user.Id;
    }
    
    public async Task IsUserInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        if (role.IsNullOrEmpty())
        {
            throw new LoggerException(ErrorMessages.RoleNotFound, 404);
        }
        
        if (!role.Equals(roleName))
        {
            throw new LoggerException(ErrorMessages.Forbidden, 403);
        }
    }
    
    public async Task<bool> HasUserInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        if (role.IsNullOrEmpty())
        {
            return false;
        }
        
        if (!role.Equals(roleName))
        {
            return false;
        }

        return true;
    }
}