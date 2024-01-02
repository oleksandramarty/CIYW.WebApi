using CIYW.Const.Enum;
using CIYW.Mediator.Mediatr.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediatr.Dictionary.Handlers;

public class DictionaryQueryHandler: IRequestHandler<DictionaryQuery, DictionariesResponse>
{
    private readonly IMediator mediator;

    public DictionaryQueryHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<DictionariesResponse> Handle(DictionaryQuery request, CancellationToken cancellationToken)
    {
        DictionariesResponse response = new DictionariesResponse();

        response.Currencies = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Currency), cancellationToken);
        response.Categories = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Category), cancellationToken);
        response.Roles = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Role), cancellationToken);
        response.Tariffs = await this.mediator.Send(new DictionaryTypeQuery(EntityTypeEnum.Tariff), cancellationToken);

        return response;
    }
}