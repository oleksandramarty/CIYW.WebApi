using System.Linq.Expressions;
using CIYW.Domain;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CIYW.Repositories;

public class JobService: IJobService
{
    private readonly DataContext context;
    private readonly IElasticSearchRepository elasticSearchRepository;
    private readonly IConfiguration configuration;
    //private static IHttpClientFactory httpClientFactory;
    //private readonly IOptions<EmailConfiguration> options;

    public JobService(
        DataContext context, 
        IElasticSearchRepository elasticSearchRepository, 
        IConfiguration configuration
        //IHttpClientFactory httpClientFactory,
        //IOptions<EmailConfiguration> options
        )
    {
        this.context = context;
        this.elasticSearchRepository = elasticSearchRepository;
        this.configuration = configuration;
    }
    
    public async Task MapUsersAsync(CancellationToken cancellationToken)
    {
        DateTime targetDate = DateTime.UtcNow.AddDays(-1);

        var schema = this.configuration["ELKConfiguration:Indexes:Users"];

        List<User> entities = await this.context.Users
            .Where(r => r.Mapped < targetDate || !r.Mapped.HasValue)
            .Take(100)
            .ToListAsync(cancellationToken);

        if (!entities.Any())
        {
            return;
        }

        foreach (var entity in entities)
        {
            if (entity.Mapped.HasValue)
            {
               this.elasticSearchRepository.DeleteById<User>(t => t.Id == entity.Id, entity.Id);                
            }
        }

        await this.elasticSearchRepository.AddEntitiesAsync<User>(entities, schema, cancellationToken);
        
        this.context.Users.UpdateRange(entities.Select(x =>
        {
            x.Mapped = DateTime.UtcNow;
            return x;
        }).ToList());
        
        await this.context.SaveChangesAsync(cancellationToken);
        
        if (await this.context.Users.AnyAsync(r => r.Mapped < targetDate, cancellationToken))
        {
            BackgroundJob.Enqueue(() => this.MapUsersAsync(cancellationToken));
        }
    }
    
    public async Task MapInvoicesAsync(CancellationToken cancellationToken)
    {
        DateTime targetDate = DateTime.UtcNow.AddDays(-1);

        var schema = this.configuration["ELKConfiguration:Indexes:Invoices"];

        List<Invoice> entities = await this.context.Invoices
            .Where(r => r.Mapped < targetDate || !r.Mapped.HasValue)
            .Take(100)
            .ToListAsync(cancellationToken);

        if (!entities.Any())
        {
            return;
        }

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
        
        if (await this.context.Invoices.AnyAsync(r => r.Mapped < targetDate, cancellationToken))
        {
            BackgroundJob.Enqueue(() => this.MapInvoicesAsync(cancellationToken));
        }
    }
}