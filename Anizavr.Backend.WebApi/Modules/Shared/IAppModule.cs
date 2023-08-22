namespace Anizavr.Backend.WebApi.Modules.Shared;

public interface IAppModule
{
    void ConfigureServices(WebApplicationBuilder builder);

    void ConfigureApplication(WebApplication app);
    
    bool Enabled { get; set; }
    
    int OrderIndex { get; set; }
}