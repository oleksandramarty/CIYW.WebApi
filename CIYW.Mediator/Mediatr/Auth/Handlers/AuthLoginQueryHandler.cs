using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using CIYW.Auth.Schemes;
using CIYW.Auth.Tokens;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediatr.Auth.Queries;
using CIYW.Models.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using AuthenticationException = CIYW.Kernel.Exceptions.AuthenticationException;

namespace CIYW.Mediator.Mediatr.Auth.Handlers;

public class AuthLoginQueryHandler: IRequestHandler<AuthLoginQuery, TokenResponse>
{
  private readonly TokenGenerator tokenGenerator;
  private readonly IAuthRepository authRepository;
  private readonly IHttpContextAccessor httpContextAccessor;

  public AuthLoginQueryHandler(IAuthRepository authRepository, TokenGenerator tokenGenerator, IHttpContextAccessor httpContextAccessor)
  {
    this.authRepository = authRepository;
    this.tokenGenerator = tokenGenerator;
    this.httpContextAccessor = httpContextAccessor;
  }

  public async Task<TokenResponse> Handle(AuthLoginQuery query, CancellationToken cancellationToken)
  {
      Guid? userId = null;
      var userHelper = await this.GetUserAsync(query);
      User user = userHelper.Item1;
      string provider = userHelper.Item2;
      
      HttpContext httpContext = this.httpContextAccessor.HttpContext;
      HttpRequest httpRequest = httpContext.Request;
      ClaimsPrincipal principal = httpContext.User;

      if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
      {
        AuthenticationHeaderValue.TryParse(httpRequest.Headers["Authorization"],
          out AuthenticationHeaderValue headerValue);
        var token = (JwtSecurityToken)(new JwtSecurityTokenHandler().ReadToken(headerValue.Parameter));
        var tokenResponse = new TokenResponse {
          Scheme = JwtCIYWDefaults.AuthenticationScheme,
          Value = headerValue.Parameter,
          Expired = token.ValidTo.ConvertToUnixTimestamp(),
          Provider = provider
        };
        return tokenResponse;
      }

      var role = await this.authRepository.GetRolesAsync(user);
      var passwordCorrect = await authRepository.CheckPasswordAsync(user, query.Password);

      if (passwordCorrect)
      {
        var tokenResponse = this.tokenGenerator.GenerateJwtToken(user.PhoneNumber, user,
          role.FirstOrDefault(), query.RememberMe, provider);
          ;
        var res = await this.authRepository.SetAuthenticationTokenAsync(user, provider,
          TokenNameProvider.CIYWAuth, (string)tokenResponse.Value);
        if (!res.Succeeded)
        {
          throw new LoggerException("Failed to save persistent token to database", 500,
            userId);
        }

        tokenResponse.Provider = provider;
        
        return tokenResponse;
      }

      throw new AuthenticationException(ErrorMessages.WrongAuth, 404, null);
    }

  private async Task<Tuple<User, string>> GetUserAsync(AuthLoginQuery query)
  {
    User user = null;
    string provider = null;
    if (query.Login.NotNullOrEmpty())
    {
      user = await this.authRepository.FindUserByLoginAsync(LoginProvider.CIYWLogin, query.Login);
      provider = LoginProvider.CIYWLogin;
    }
    if (query.Phone.NotNullOrEmpty())
    {
      user = await this.authRepository.FindUserByLoginAsync(LoginProvider.CIYWPhone, query.Phone);
      provider = LoginProvider.CIYWPhone;
    }
    if (query.Email.NotNullOrEmpty())
    {
      user = await this.authRepository.FindUserByLoginAsync(LoginProvider.CIYWEmail, query.Email);
      provider = LoginProvider.CIYWEmail;
    }
    if (user == null)
    {
      throw new AuthenticationException(ErrorMessages.UserNotFound, 404,
        null);
    }
    return new Tuple<User, string>(user, provider);
  }
}