using CIYW.Const.Enum;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediatr.Dictionary.Requests;

public class DictionaryTypeQuery: IRequest<IList<DictionaryItemResponse>>
{
    public DictionaryTypeQuery(EntityTypeEnum type)
    {
        Type = type;
    }

    public EntityTypeEnum Type { get; set; }
}