using System.Security.Claims;

namespace CIYW.Auth;

public static class ClaimsPrincipalExtension
{
    public static Guid GetUserId(this ClaimsPrincipal source)
    {
        var value = source.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        Guid guid;
        if (Guid.TryParse(value, out guid))
        {
            return guid;
        }
        return Guid.Empty;
    }
}