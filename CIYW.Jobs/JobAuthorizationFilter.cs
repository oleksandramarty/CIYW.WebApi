using Hangfire.Dashboard;

namespace CIYW.Jobs.Jobs;

public class JobAuthorizationFilter: IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
#if DEBUG
        return true;
#endif
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}