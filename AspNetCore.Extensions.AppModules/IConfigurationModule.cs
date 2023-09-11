using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Extensions.AppModules;

public interface IConfigurationModule
{
    void ConfigureServices(WebApplicationBuilder builder);
    bool Enabled => true;
}

