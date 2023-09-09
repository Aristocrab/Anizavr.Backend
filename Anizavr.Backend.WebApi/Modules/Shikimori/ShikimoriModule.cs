using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.ShikimoriApi;
using Anizavr.Backend.WebApi.Configuration;
using NSubstitute;
using Refit;
using ShikimoriSharp;
using ShikimoriSharp.Bases;

namespace Anizavr.Backend.WebApi.Modules.Shikimori;

public class ShikimoriModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IWebApiConfiguration>();
        
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(
            configuration.ShikimoriClientName, 
            configuration.ShikimoriClientId, 
            configuration.ShikimoriClientKey);
        var client = new ShikimoriClient(logger, settings);
        builder.Services.AddSingleton<IShikimoriClient>(new ShikimoriClientAdapter(client));

        builder.Services.AddSingleton(RestService.For<IShikimoriApi>("https://shikimori.one/api"));
    }
}