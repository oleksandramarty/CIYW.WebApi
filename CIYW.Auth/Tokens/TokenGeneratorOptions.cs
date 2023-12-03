namespace CIYW.Auth.Tokens;

public class TokenGeneratorOptions
{
    public string JwtKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtExpireDays { get; set; }
}