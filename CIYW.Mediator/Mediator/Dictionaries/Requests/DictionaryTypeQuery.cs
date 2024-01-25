using CIYW.Const.Enums;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Requests;

public class DictionaryTypeQuery: IRequest<DictionaryResponse<Guid>>
{
    public DictionaryTypeQuery(DictionaryTypeEnum type)
    {
        Type = type;
    }

    public DictionaryTypeEnum Type { get; set; }
}