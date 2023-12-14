using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CIYW.Const.Providers;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CIYW.Auth.Schemes;

public class JwtCIYWHandler : AuthenticationHandler<JwtCIYWOptions>
    {
        private IAuthRepository _authRepository;
        public JwtCIYWHandler(
            IOptionsMonitor<JwtCIYWOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthRepository authRepository
            ) : base(options, logger, encoder, clock)
        {
            _authRepository = authRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                if (!Request.Query.ContainsKey("access_token"))
                {
                    return AuthenticateResult.NoResult();
                }
                else
                {
                    Request.Headers.Add("Authorization", $"{JwtCIYWDefaults.BearerScheme} { Request.Query["access_token"]}");
                }
            }
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }
            if (!(JwtCIYWDefaults.AuthenticationScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase)
                || JwtCIYWDefaults.BearerScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase)))
            {
                return AuthenticateResult.NoResult();
            }
            var token = (JwtSecurityToken)(new JwtSecurityTokenHandler().ReadToken(headerValue.Parameter));
            if(token.ValidTo<=DateTime.UtcNow)
            {
                return AuthenticateResult.NoResult();
            }

            var userManager = (UserManager<User>)Context.RequestServices.GetService(typeof(UserManager<User>));
            var uid = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (uid == null) return AuthenticateResult.Fail("В токене не найден идентификатор пользователя");

            var user = await this._authRepository.FindUserByIdAsync(uid.Value);
            if (user ==  null) return AuthenticateResult.NoResult();
            if (user.IsBlocked) return AuthenticateResult.NoResult();
            var provider = token.Claims.FirstOrDefault(claim => claim.Type == "provider");
            var savedTokenPhone = await this._authRepository.GetAuthenticationTokenAsync(user, provider?.Value ?? LoginProvider.CIYWPhone, TokenNameProvider.CIYWAuth);
            if (savedTokenPhone == null) return AuthenticateResult.NoResult();
            if (savedTokenPhone != headerValue.Parameter) return AuthenticateResult.NoResult();

            var claims = token.Claims.ToArray();
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["Authenticate"] = $"CIYW realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }