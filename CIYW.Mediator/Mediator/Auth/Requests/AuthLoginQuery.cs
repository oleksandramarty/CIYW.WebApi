using CIYW.Models.Responses.Auth;
using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Queries;

public class AuthLoginQuery: IRequest<TokenResponse>
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}