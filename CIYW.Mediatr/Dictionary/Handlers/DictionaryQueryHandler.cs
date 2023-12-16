using CIYW.Const.Enum;
using CIYW.Mediatr.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediatr.Dictionary.Handlers;

public class DictionaryQueryHandler: IRequestHandler<DictionaryQuery, DictionaryResponse>
{
    private readonly IMediator mediator;

    public DictionaryQueryHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<DictionaryResponse> Handle(DictionaryQuery request, CancellationToken cancellationToken)
    {
        DictionaryResponse response = new DictionaryResponse();

        response.Currencies = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Currency), cancellationToken);
        response.Categories = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Category), cancellationToken);
        response.Roles = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Role), cancellationToken);
        response.Tariffs = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Tariff), cancellationToken);

        return response;
    }
}