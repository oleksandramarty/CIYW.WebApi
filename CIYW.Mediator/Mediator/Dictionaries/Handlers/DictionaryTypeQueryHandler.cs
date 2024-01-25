using AutoMapper;
using CIYW.Const.Enums;
using CIYW.Const.Errors;
using CIYW.Domain.Models.Categories;
using CIYW.Domain.Models.Currencies;
using CIYW.Domain.Models.Tariffs;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediator.Mediator.Dictionaries.Requests;
using CIYW.Models.Responses.Dictionaries;
using MediatR;

namespace CIYW.Mediator.Mediator.Dictionaries.Handlers;

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
            case DictionaryTypeEnum.CURRENCY:
                return await this.GetCurrencies(cancellationToken);
            case DictionaryTypeEnum.ROLE:
                return await this.GetRoles(cancellationToken);
            case DictionaryTypeEnum.TARIFF:
                return await this.GetTariffs(cancellationToken);
            case DictionaryTypeEnum.CATEGORY:
                return await this.GetCategories(cancellationToken);
            default:
                throw new LoggerException(ErrorMessages.DictionaryNotFound, 404);
        }
    }

    private async Task<IList<DictionaryItemResponse<Guid>>> GetCurrencies(CancellationToken cancellationToken)
    {
        IList<Currency> items = await this.dictionaryRepository.GetAllAsync<Currency>(cancellationToken);
        return items.Select(x => this.mapper.Map<Currency, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetRoles(CancellationToken cancellationToken)
    {
        IList<Role> items = await this.dictionaryRepository.GetAllAsync<Role>(cancellationToken);
        return items.Select(x => this.mapper.Map<Role, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetTariffs(CancellationToken cancellationToken)
    {
        IList<Tariff> items = await this.dictionaryRepository.GetAllAsync<Tariff>(cancellationToken);
        return items.Select(x => this.mapper.Map<Tariff, DictionaryItemResponse<Guid>>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse<Guid>>> GetCategories(CancellationToken cancellationToken)
    {
        IList<Category> items = await this.dictionaryRepository.GetAllAsync<Category>(cancellationToken);
        return items.Select(x => this.mapper.Map<Category, DictionaryItemResponse<Guid>>(x)).ToList();
    }
}