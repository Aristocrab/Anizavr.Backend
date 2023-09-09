using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.WebApi.Configuration;
using Refit;

namespace Anizavr.Backend.WebApi.Modules.Kodik;

public class KodikModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IWebApiConfiguration>();
        
        var kodikApi = RestService.For<IKodikApi>("https://kodikapi.com");
        var kodikService = new KodikApiAdapter(kodikApi, configuration.KodikKey);
        builder.Services.AddSingleton<IKodikService>(kodikService);
    }
}