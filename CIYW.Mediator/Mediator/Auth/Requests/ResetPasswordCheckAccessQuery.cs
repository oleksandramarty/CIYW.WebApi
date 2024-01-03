using MediatR;

namespace CIYW.Mediator.Mediator.Auth.Requests;

public class ResetPasswordCheckAccessQuery: IRequest
{
    public ResetPasswordCheckAccessQuery(string url)
    {
        Url = Url ?? throw new ArgumentNullException(nameof(Url));;
    }

    public string Url { get; set; }
}