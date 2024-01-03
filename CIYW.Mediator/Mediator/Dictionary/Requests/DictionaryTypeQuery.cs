using CIYW.Const.Enum;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionary.Requests;

public class DictionaryTypeQuery: IRequest<DictionaryResponse>
{
    public DictionaryTypeQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}