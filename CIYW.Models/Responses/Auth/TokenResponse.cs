namespace CIYW.Models.Responses.Auth;

public class TokenResponse
{
    public string Scheme { get; set; }
    public object Value { get; set; }
    public double Expired { get; set; }
}