using AspNetCore.Extensions.AppModules;
using Serilog;
using Serilog.Events;

namespace Anizavr.Backend.WebApi.Modules.Serilog;

public class SerilogModule : AppModule
{
    public override int OrderIndex => -1;

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
#if DEBUG
            .MinimumLevel.Override("AspNetCore.Extensions.AppModules.AppModule", LogEventLevel.Debug)
#endif
            .WriteTo.Console()
            .CreateLogger();
        
        builder.Host.UseSerilog();
    }
}