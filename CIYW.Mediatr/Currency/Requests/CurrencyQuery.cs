using CIYW.Models.Responses.Currency;
using MediatR;

namespace CIYW.Mediatr.Currency.Requests;

public class CurrencyQuery: IRequest<CurrencyResponse>
{
    public CurrencyQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}