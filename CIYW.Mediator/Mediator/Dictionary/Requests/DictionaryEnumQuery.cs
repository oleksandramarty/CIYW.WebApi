using CIYW.Const.Enum;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionary.Requests;

public class DictionaryEnumQuery: IRequest<DictionaryResponse<string>>
{
    public DictionaryEnumQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}