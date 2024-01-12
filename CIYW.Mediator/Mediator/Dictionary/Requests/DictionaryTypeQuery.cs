using CIYW.Const.Enums;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionary.Requests;

public class DictionaryTypeQuery: IRequest<DictionaryResponse<Guid>>
{
    public DictionaryTypeQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}