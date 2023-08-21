using System.Diagnostics;
using Calabonga.AspNetCore.AppDefinitions;
using Serilog;

namespace Anizavr.Backend.WebApi.Definitions.RequestLogging;

public class RequestLoggingDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            Log.Information("{RequestMethod} {RequestPath} started", 
                context.Request.Method, 
                context.Request.Path);
            
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            await next(context);
            stopwatch.Stop();
            
            Log.Information("{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms",
                context.Request.Method, 
                context.Request.Path, 
                context.Response.StatusCode, 
                stopwatch.Elapsed.TotalMilliseconds);
        });
    }
}