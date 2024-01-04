using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CIYW.Auth.Schemes;
using CIYW.Domain.Models.User;
using CIYW.Kernel.Extensions;
using CIYW.Models.Responses.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CIYW.Auth.Tokens;

  public class TokenGenerator
    {
        private TokenGeneratorOptions _options;
        public TokenGenerator (IOptions<TokenGeneratorOptions> options)
        {
            _options = options.Value;
        }

        public TokenResponse GenerateJwtToken(
          string phone,
          User user,
          string roleName,
          bool remember,
          string provider,
          int duration = 1)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthParams.Strings.JwtRegisteredClaimNames.Sub, phone),
                new Claim(AuthParams.Strings.JwtRegisteredClaimNames.Provider, provider),
                new Claim(AuthParams.Strings.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roleName )
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(remember ? 7 : GetExpiredDays());

            var token = new JwtSecurityToken(
                _options.JwtIssuer,
                _options.JwtIssuer,
                claims,

                expires: expires,
                signingCredentials: creds
            );
            var gen = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse
            {
                Scheme = JwtCIYWDefaults.AuthenticationScheme,
                Value = gen,
                Expired = expires.ConvertToUnixTimestamp()
            };
        }

        private double GetExpiredDays()
        {
            var str = _options.JwtExpireDays;
            return str.NotNullOrEmpty() ? Convert.ToDouble(str) : 1.0;
        }

    }