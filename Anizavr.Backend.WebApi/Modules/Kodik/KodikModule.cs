using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.WebApi.Configuration;
using AspNetCore.Extensions.AppModules.ModuleTypes;
using Refit;

namespace Anizavr.Backend.WebApi.Modules.Kodik;

public class KodikModule : AppModule
{
    private readonly IWebApiConfiguration _configuration;

    public KodikModule(IWebApiConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var kodikApi = RestService.For<IKodikApi>("https://kodikapi.com");
        var kodikService = new KodikApiAdapter(kodikApi, _configuration.KodikKey);
        builder.Services.AddSingleton<IKodikClient>(kodikService);
    }
}