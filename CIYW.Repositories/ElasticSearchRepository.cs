using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Utils;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace CIYW.Repositories;

public class ElasticSearchRepository: IElasticSearchRepository
{
    private readonly IElasticClient elasticClient;
    private readonly IMapper mapper;
    private readonly DataContext context;

    public ElasticSearchRepository(
        IElasticClient elasticClient, 
        IMapper mapper,
        DataContext context)
    {
        this.elasticClient = elasticClient;
        this.mapper = mapper;
        this.context = context;
    }

    public async Task MapEntityAsync<T, TMapped>(T entity, CancellationToken cancellationToken)
        where T: class
        where TMapped: class
    {
        Guid entityId = ReflectionUtils.GetValue<T, Guid>(entity, "Id");

        TMapped mappedEntity = this.mapper.Map<T, TMapped>(entity);

        await this.AddOrUpdateEntityAsync<TMapped>(e => ReflectionUtils.GetValue<TMapped, Guid>(e, "Id") == entityId, entityId, mappedEntity, cancellationToken);
        
        ReflectionUtils.SetValue<T, DateTime?>(entity, "Mapped", DateTime.UtcNow);

        this.context.Set<T>().Update(entity);
        await this.context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddEntityAsync<T>(T entity, CancellationToken cancellationToken) where T: class
    {
        var response = await this.elasticClient.IndexDocumentAsync(entity, cancellationToken);
        this.CheckResponse(response.IsValid, response.DebugInformation);
    }
    
    public async Task AddEntitiesAsync<T>(List<T> entities, string schemeName, CancellationToken cancellationToken) where T : class
    {
        var response = await this.elasticClient.IndexManyAsync<T>(entities, schemeName, cancellationToken);
        this.CheckResponse(response.IsValid, response.DebugInformation);
    }

    public async Task AddOrUpdateEntityAsync<T>(Expression<Func<T, bool>> predicate, Guid id, T entity,
        CancellationToken cancellationToken) where T : class
    {
        var temp = await this.GetByIdAsync<T>(predicate, id, cancellationToken);

        if (temp == null)
        {
           await this.AddEntityAsync<T>(entity, cancellationToken);
        }
        else
        {
            await this.UpdateEntityAsync(predicate, id, entity, cancellationToken);
        }
    }
    
    public async Task UpdateEntityAsync<T>(Expression<Func<T, bool>> predicate, Guid id, T entity, CancellationToken cancellationToken) where T: class
    {
        this.DeleteById(predicate, id);
        await this.AddEntityAsync<T>(entity, cancellationToken);
    }

    public void DeleteById<T>(Expression<Func<T, bool>> predicate, Guid id) where T: class
    {
        var response = this.elasticClient.DeleteByQuery<T>(p => p.Query(q1 => q1
            .Match(m => m
                .Field(predicate)
                .Query(id.ToString())
            )));
        this.CheckResponse(response.IsValid, response.DebugInformation);
    }
    
    public async Task<T?> GetByIdAsync<T>(Expression<Func<T, bool>> predicate, Guid id, CancellationToken cancellationToken) where T: class
    {
        var response = await this.elasticClient.SearchAsync<T>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(predicate)
                    .Query(id.ToString())
                )
            )
            .Size(1), cancellationToken);
        
        this.CheckResponse(response.IsValid, response.DebugInformation);
        
        return response.Documents.FirstOrDefault();
    }
    
    public async Task<ListWithIncludeHelper<T>> GetPaginatedResultsAsync<T>(Expression<Func<T, bool>> userIdPredicate,
        Guid userId,
        BaseFilterQuery filter,
        CancellationToken cancellationToken) where T : class
    {
        int skip = (filter.Paginator.PageNumber - 1) * filter.Paginator.PageSize;
        var result = await this.elasticClient.SearchAsync<T>(s => s
                .Query(q => q
                    .Match(m => m
                            .Field(userIdPredicate)
                            .Query(userId.ToString())
                    )
                ).Sort(sort =>
                {
                    if (sort != null)
                    {
                        return filter.Sort.Direction == "asc" ?
                            sort.Ascending(t => EF.Property<object>(t, filter.Sort.Column)) :
                            sort.Descending(t => EF.Property<object>(t, filter.Sort.Column));
                    }
                    return null;
                })
                .From(skip).Size(filter.Paginator.PageSize)
        );

        if (!result.IsValid)
        {
            return new ListWithIncludeHelper<T>
            {
                Entities = new List<T>(),
                Paginator = filter.Paginator,
                TotalCount = 0
            };
        }
        
        var total = await this.elasticClient.CountAsync<T>(t => t.AllIndices(), cancellationToken);

        List<T> entities = result.Documents.ToList();

        return new ListWithIncludeHelper<T>
        {
            Entities = entities,
            Paginator = filter.Paginator,
            TotalCount = (int)total.Count
        };
    }

    private void CheckResponse(bool isValid, string info)
    {
        if (!isValid)
        {
            throw new LoggerException(String.Format(ErrorMessages.ElasticSearchError, info), 409);            
        }
    }
}