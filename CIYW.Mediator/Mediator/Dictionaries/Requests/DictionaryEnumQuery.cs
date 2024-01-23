using CIYW.Const.Enums;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Requests;

public class DictionaryEnumQuery: IRequest<DictionaryResponse<string>>
{
    public DictionaryEnumQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}