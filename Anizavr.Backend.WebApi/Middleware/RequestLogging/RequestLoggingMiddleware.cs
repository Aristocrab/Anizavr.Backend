using System.Diagnostics;
using Serilog;

namespace Anizavr.Backend.WebApi.Middleware.RequestLogging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = new Stopwatch();
        sw.Start();
        await _next(context);
        sw.Stop();
        var elapsed = sw.Elapsed.TotalMilliseconds;
        
        Log.Information("{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed);
    }
}