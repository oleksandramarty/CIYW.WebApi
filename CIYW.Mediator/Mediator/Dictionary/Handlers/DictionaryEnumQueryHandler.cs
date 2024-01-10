using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionary.Handlers;

public class DictionaryEnumQueryHandler: IRequestHandler<DictionaryEnumQuery, DictionaryResponse<string>>
{
    public async Task<DictionaryResponse<string>> Handle(DictionaryEnumQuery query, CancellationToken cancellationToken)
    {
        switch (query.Type)
        {
            case EntityTypeEnum.InvoiceType:
                return EnumExtension.ConvertEnumToDictionary<InvoiceTypeEnum>();
            default:
                throw new LoggerException(ErrorMessages.DictionaryNotFound, 404);
        }
    }
}