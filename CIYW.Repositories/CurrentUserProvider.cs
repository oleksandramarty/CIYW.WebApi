using System.Security.Claims;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Repositories;

public class CurrentUserProvider: ICurrentUserProvider
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;

    public CurrentUserProvider(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    
    public async Task<Guid> GetUserIdAsync(CancellationToken cancellationToken)
    {
        // Try to get user ID from HttpContext
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
            return Guid.Parse(userId);
        }

        // If HttpContext is not available or user ID is not found, fall back to UserManager
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

        if (user == null)
        {
            throw new LoggerException(ErrorMessages.UserNotFound, 404);
        }
        
        return user.Id;
    }
}