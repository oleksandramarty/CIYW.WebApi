using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionary.Handlers;

public class DictionaryTypeQueryHandler: IRequestHandler<DictionaryTypeQuery, DictionaryResponse<Guid>>
{
    private readonly IMapper mapper;
    private readonly IDictionaryRepository dictionaryRepository;

    public DictionaryTypeQueryHandler(IMapper mapper, IDictionaryRepository dictionaryRepository)
    {
        this.mapper = mapper;
        this.dictionaryRepository = dictionaryRepository;
    }

    public async Task<DictionaryResponse<Guid>> Handle(DictionaryTypeQuery query, CancellationToken cancellationToken)
    {

        IList<DictionaryItemResponse<Guid>> items = await this.GetItems(query, cancellationToken);
        return new DictionaryResponse<Guid>(items);
    }

    private async Task<IList<DictionaryItemResponse<Guid>>> GetItems(DictionaryTypeQuery query, CancellationToken cancellationToken)
    {
        switch (query.Type)
        {
            case EntityTypeEnum.Currency:
                return await this.GetCurrencies(cancellationToken);
            case EntityTypeEnum.Role:
                return await this.GetRoles(cancellationToken);
            case EntityTypeEnum.Tariff:
                return await this.GetTariffs(cancellationToken);
            case EntityTypeEnum.Category:
                return await this.GetCategories(cancellationToken);
            default:
                throw new LoggerException(ErrorMessages.DictionaryNotFound, 404);
        }
    }

    private async Task<IList<DictionaryItemResponse<Guid>>> GetCurrencies(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Currency.Currency> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Currency.Currency>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Currency.Currency, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetRoles(CancellationToken cancellationToken)
    {
        IList<Role> items = await this.dictionaryRepository.GetAllAsync<Role>(cancellationToken);
        return items.Select(x => this.mapper.Map<Role, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetTariffs(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Tariff.Tariff> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Tariff.Tariff>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Tariff.Tariff, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetCategories(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Category.Category> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Category.Category>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Category.Category, DictionaryItemResponse<Guid>>(x)).ToList();
    }
}