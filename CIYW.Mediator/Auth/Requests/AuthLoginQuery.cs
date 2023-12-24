using System.Security.Claims;
using CIYW.Models.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CIYW.Mediator.Auth.Queries;

public class AuthLoginQuery: IRequest<TokenResponse>
{
    public AuthLoginQuery(string login, string email, string phone, string password, bool rememberMe)
    {
        Login = login;
        Email = email;
        Phone = phone;
        Password = password;
        RememberMe = rememberMe;
    }

    public string Login { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}