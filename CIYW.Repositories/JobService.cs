using AutoMapper;
using CIYW.Domain;
using CIYW.Domain.Models.Invoices;
using CIYW.Domain.Models.Users;
using CIYW.Elasticsearch.Models.Users;
using CIYW.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CIYW.Repositories;

public class JobService: IJobService
{
    private readonly DataContext context;
    private readonly IElasticSearchRepository elasticSearchRepository;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    //private static IHttpClientFactory httpClientFactory;
    //private readonly IOptions<EmailConfiguration> options;

    public JobService(
        DataContext context, 
        IElasticSearchRepository elasticSearchRepository, 
        IMapper mapper,
        IConfiguration configuration
        //IHttpClientFactory httpClientFactory,
        //IOptions<EmailConfiguration> options
        )
    {
        this.context = context;
        this.elasticSearchRepository = elasticSearchRepository;
        this.mapper = mapper;
        this.configuration = configuration;
    }
    
    public async Task MapUsersAsync(CancellationToken cancellationToken)
    {
        DateTime targetDate = DateTime.UtcNow.AddDays(-1);

        List<User> entities = await this.context.Users
            .Include(u => u.UserBalance)
            .ThenInclude(u => u.Currency)
            //.Where(r => r.Mapped < targetDate || !r.Mapped.HasValue)
            .Take(500)
            .ToListAsync(cancellationToken);

        if (!entities.Any())
        {
            return;
        }
        
        bool isNeed = entities.Any();
        
        var schema = this.configuration["ELKConfiguration:Indexes:Users"];

        List<UserSearchModel> mappedEntities = new List<UserSearchModel>();
        
        while (isNeed)
        {
            foreach (var entity in entities)
            {
                if (entity.Mapped.HasValue)
                {
                    this.elasticSearchRepository.DeleteById<UserSearchModel>(t => t.Id == entity.Id, entity.Id);                
                }
                mappedEntities.Add(this.mapper.Map<User, UserSearchModel>(entity));
                
                // await this.elasticSearchRepository.MapEntityAsync<Users, UserSearchModel>(entity, cancellationToken);
            }
            
            await this.elasticSearchRepository.AddEntitiesAsync<UserSearchModel>(mappedEntities, schema, cancellationToken);
        
            this.context.Users.UpdateRange(entities.Select(x =>
            {
                x.Mapped = DateTime.UtcNow;
                return x;
            }).ToList());
        
            await this.context.SaveChangesAsync(cancellationToken);
            
            entities = await this.context.Users
                .Where(r => r.Mapped < targetDate || !r.Mapped.HasValue)
                .Take(500)
                .ToListAsync(cancellationToken);
            isNeed = entities.Any();
        }
    }
    
    public async Task MapInvoicesAsync(CancellationToken cancellationToken)
    {
        DateTime targetDate = DateTime.UtcNow.AddDays(-1);
        DateTime monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        List<Invoice> entities = await this.context.Invoices
            .Where(r => (r.Mapped < targetDate || !r.Mapped.HasValue) && r.Created >= monthStart)
            .Take(100)
            .ToListAsync(cancellationToken);

        if (!entities.Any())
        {
            return;
        }

        bool isNeed = entities.Any();
        
        var schema = this.configuration["ELKConfiguration:Indexes:Invoices"];

        while (isNeed)
        {
            foreach (var entity in entities)
            {
                if (entity.Mapped.HasValue)
                {
                    this.elasticSearchRepository.DeleteById<Invoice>(t => t.Id == entity.Id, entity.Id);                
                }
            }

            await this.elasticSearchRepository.AddEntitiesAsync<Invoice>(entities, schema, cancellationToken);
        
            this.context.Invoices.UpdateRange(entities.Select(x =>
            {
                x.Mapped = DateTime.UtcNow;
                return x;
            }).ToList());
        
            await this.context.SaveChangesAsync(cancellationToken);
            
            entities = await this.context.Invoices
                .Where(r => (r.Mapped < targetDate || !r.Mapped.HasValue) && r.Created >= monthStart)
                .Take(100)
                .ToListAsync(cancellationToken);
            isNeed = entities.Any();
        }
    }
}