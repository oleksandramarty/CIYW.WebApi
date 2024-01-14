using CIYW.Const.Const;
using CIYW.Domain;
using CIYW.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CIYW.Jobs.Jobs;

public static class JobProvider
{
    public static void InitJobs(this IApplicationBuilder app, CancellationToken cancellationToken)
    {
        using (var serviceScope = app.ApplicationServices
                   .GetRequiredService<IServiceScopeFactory>()
                   .CreateScope())
        {
            using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
            {
                RecurringJob.AddOrUpdate<IJobService>(service => service.MapUsersAsync(cancellationToken), CronConst.CronEvery12hours);
                // RecurringJob.AddOrUpdate<IJobService>(service => service.MapInvoicesAsync(cancellationToken), CronConst.CronEvery12hours);
            }
        }
    }
}