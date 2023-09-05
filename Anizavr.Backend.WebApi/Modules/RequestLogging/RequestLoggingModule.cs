using System.Diagnostics;
using AspNetCore.AppModules;
using Serilog;

namespace Anizavr.Backend.WebApi.Modules.RequestLogging;

public class RequestLoggingModule : AppModule
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