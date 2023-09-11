using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Extensions.AppModules.ModuleTypes;

public abstract class AppModule
{
    public virtual void ConfigureServices(WebApplicationBuilder builder) { }

    public virtual void ConfigureApplication(WebApplication app) { }

    public virtual bool Enabled { get; set; } = true;
    
    public virtual int OrderIndex { get; set; }
}