using Aristocrab.AppModules;
using Serilog;
using Serilog.Events;

namespace Anizavr.Backend.WebApi.Modules.Serilog;

public class SerilogModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var formatWithNamespace = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: formatWithNamespace)
            .CreateLogger();
        
        builder.Host.UseSerilog();
    }
}