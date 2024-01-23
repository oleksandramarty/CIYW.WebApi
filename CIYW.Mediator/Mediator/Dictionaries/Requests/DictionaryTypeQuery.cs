using CIYW.Const.Enums;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Requests;

public class DictionaryTypeQuery: IRequest<DictionaryResponse<Guid>>
{
    public DictionaryTypeQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}