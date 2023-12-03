using System.Diagnostics;

namespace CIYW.ClientApi.Middleware;

public class RequestTimeMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimeMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        // Игнорируем Swagger (не логируем)
        if (context.Request.Path.StartsWithSegments("/swagger", StringComparison.InvariantCultureIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/hangfire", StringComparison.InvariantCultureIgnoreCase))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();

        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext) state;
            httpContext.Response.Headers.Add("X-Response-Time-Milliseconds",
                new[] {sw.ElapsedMilliseconds.ToString()});
            return Task.FromResult(0);
        }, context);

        await _next(context);
    }
}