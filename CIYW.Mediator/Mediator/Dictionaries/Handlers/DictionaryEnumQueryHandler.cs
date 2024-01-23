using CIYW.Const.Enums;
using CIYW.Const.Errors;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Handlers;

public class DictionaryEnumQueryHandler: IRequestHandler<DictionaryEnumQuery, DictionaryResponse<string>>
{
    public async Task<DictionaryResponse<string>> Handle(DictionaryEnumQuery query, CancellationToken cancellationToken)
    {
        switch (query.Type)
        {
            case EntityTypeEnum.INVOICE_TYPE:
                return EnumExtension.ConvertEnumToDictionary<InvoiceTypeEnum>();
            default:
                throw new LoggerException(ErrorMessages.DictionaryNotFound, 404);
        }
    }
}