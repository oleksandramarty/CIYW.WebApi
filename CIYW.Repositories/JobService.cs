using CIYW.Domain;
using CIYW.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class JobService: IJobService
{
    private readonly DataContext context;
    //private static IHttpClientFactory httpClientFactory;
    //private readonly IOptions<EmailConfiguration> options;

    public JobService(
        DataContext context
        //IHttpClientFactory httpClientFactory,
        //IOptions<EmailConfiguration> options
        )
    {
        this.context = context;
    }
    
    public async Task TestJobAsync(CancellationToken cancellationToken)
    {
        var categories = await this.context.Categories.ToListAsync();
        var currencies = await this.context.Currencies.ToListAsync();
    }
}