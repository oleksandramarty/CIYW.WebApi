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
using CIYW.Mediatr.Auth.Queries;
using CIYW.Models.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using AuthenticationException = CIYW.Kernel.Exceptions.AuthenticationException;

namespace CIYW.Mediatr.Auth.Handlers;

public class AuthLoginQueryHandler: IRequestHandler<AuthLoginQuery, TokenResponse>
{
  private readonly TokenGenerator _tokenGenerator;
  private readonly IAuthService _authService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuthLoginQueryHandler(IAuthService authService, TokenGenerator tokenGenerator, IHttpContextAccessor httpContextAccessor)
  {
    _authService = authService;
    _tokenGenerator = tokenGenerator;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<TokenResponse> Handle(AuthLoginQuery query, CancellationToken cancellationToken)
  {
      Guid? userId = null;
      var userHelper = await this.GetUserAsync(query);
      User user = userHelper.Item1;
      string provider = userHelper.Item2;
      
      HttpContext httpContext = this._httpContextAccessor.HttpContext;
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
          Expired = token.ValidTo.ConvertToUnixTimestamp()
        };
        return tokenResponse;
      }

      var role = await this._authService.GetRolesAsync(user);
      var passwordCorrect = await _authService.CheckPasswordAsync(user, query.Password);

      if (passwordCorrect)
      {
        var tokenResponse = this._tokenGenerator.GenerateJwtToken(user.PhoneNumber, user,
          role.FirstOrDefault(), query.RememberMe, provider);
          ;
        var res = await _authService.SetAuthenticationTokenAsync(user, provider,
          TokenNameProvider.CIYWAuth, (string)tokenResponse.Value);
        if (!res.Succeeded)
        {
          throw new LoggerException("Failed to save persistent token to database", 500,
            userId, EntityTypeEnum.Session.ToString());
        }

        return tokenResponse;
      }

      throw new AuthenticationException(ErrorMessages.UserNotFound, 404, null, EntityTypeEnum.User.ToString());
    }

  private async Task<Tuple<User, string>> GetUserAsync(AuthLoginQuery query)
  {
    User user = null;
    string provider = null;
    if (query.Login.NotNullOrEmpty())
    {
      user = await this._authService.FindUserByLoginAsync(LoginProvider.CIYWLogin, query.Login);
      provider = LoginProvider.CIYWLogin;
    }
    if (query.Phone.NotNullOrEmpty())
    {
      user = await this._authService.FindUserByLoginAsync(LoginProvider.CIYWPhone, query.Phone);
      provider = LoginProvider.CIYWPhone;
    }
    if (query.Email.NotNullOrEmpty())
    {
      user = await this._authService.FindUserByLoginAsync(LoginProvider.CIYWEmail, query.Email);
      provider = LoginProvider.CIYWEmail;
    }
    if (user == null)
    {
      throw new LoggerException(ErrorMessages.UserNotFound, 404,
        null, EntityTypeEnum.Session.ToString());
    }
    return new Tuple<User, string>(user, provider);
  }
}