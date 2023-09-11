using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Extensions.AppModules.ModuleTypes;

public abstract class ConfigurationAppModule
{
    public virtual void ConfigureServices(WebApplicationBuilder builder) { }

    public virtual bool Enabled { get; set; } = true;
    
    public virtual int OrderIndex { get; set; }
}

