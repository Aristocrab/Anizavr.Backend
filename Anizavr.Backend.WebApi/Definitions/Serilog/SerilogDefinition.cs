using Calabonga.AspNetCore.AppDefinitions;
using Serilog;
using Serilog.Events;

namespace Anizavr.Backend.WebApi.Definitions.Serilog;

public class SerilogDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console()
            .CreateLogger();
        
        builder.Host.UseSerilog();
    }
}