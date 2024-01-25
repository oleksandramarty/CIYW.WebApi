using CIYW.Const.Enums;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Requests;

public class DictionaryEnumQuery: IRequest<DictionaryResponse<string>>
{
    public DictionaryEnumQuery(DictionaryTypeEnum type)
    {
        Type = type;
    }

    public DictionaryTypeEnum Type { get; set; }
}