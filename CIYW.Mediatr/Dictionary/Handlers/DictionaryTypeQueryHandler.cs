using System.Reflection;
using AutoMapper;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Mediatr.Dictionary.Requests;
using CIYW.Models.Responses.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediatr.Dictionary.Handlers;

public class DictionaryTypeQueryHandler: IRequestHandler<DictionaryTypeQuery, IList<DictionaryItemResponse>>
{
    private readonly IMapper mapper;
    private readonly IDictionaryRepository dictionaryRepository;

    public DictionaryTypeQueryHandler(IMapper mapper, IDictionaryRepository dictionaryRepository)
    {
        this.mapper = mapper;
        this.dictionaryRepository = dictionaryRepository;
    }

    public async Task<IList<DictionaryItemResponse>> Handle(DictionaryTypeQuery typeQuery, CancellationToken cancellationToken)
    {

        switch (typeQuery.Type)
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

    private async Task<IList<DictionaryItemResponse>> GetCurrencies(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Currency.Currency> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Currency.Currency>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Currency.Currency, DictionaryItemResponse>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse>> GetRoles(CancellationToken cancellationToken)
    {
        IList<Role> items = await this.dictionaryRepository.GetAllAsync<Role>(cancellationToken);
        return items.Select(x => this.mapper.Map<Role, DictionaryItemResponse>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse>> GetTariffs(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Tariff.Tariff> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Tariff.Tariff>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Tariff.Tariff, DictionaryItemResponse>(x)).ToList();
    }
    
    private async Task<IList<DictionaryItemResponse>> GetCategories(CancellationToken cancellationToken)
    {
        IList<Domain.Models.Category.Category> items = await this.dictionaryRepository.GetAllAsync<Domain.Models.Category.Category>(cancellationToken);
        return items.Select(x => this.mapper.Map<Domain.Models.Category.Category, DictionaryItemResponse>(x)).ToList();
    }
}