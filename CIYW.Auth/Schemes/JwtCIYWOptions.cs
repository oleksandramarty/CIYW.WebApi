using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CIYW.Auth.Schemes;

public class JwtCIYWOptions : AuthenticationSchemeOptions
{
    public string Realm { get; set; }
}

public class JwtNbxPostConfigureOptions : IPostConfigureOptions<JwtCIYWOptions>
{
    public void PostConfigure(string name, JwtCIYWOptions options)
    {
        if (string.IsNullOrEmpty(options.Realm))
        {
            throw new InvalidOperationException("Realm must be provided in options");
        }
    }
}