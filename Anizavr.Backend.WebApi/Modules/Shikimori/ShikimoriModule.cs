using Anizavr.Backend.Application.Shared;
using Anizavr.Backend.Application.ShikimoriApi;
using Aristocrab.AspNetCore.AppModules;
using NSubstitute;
using Refit;
using ShikimoriSharp;
using ShikimoriSharp.Bases;

namespace Anizavr.Backend.WebApi.Modules.Shikimori;

public class ShikimoriModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(Constants.ShikimoriClientName, 
            Constants.ShikimoriClientId, Constants.ShikimoriClientKey);
        var client = new ShikimoriClient(logger, settings);
        builder.Services.AddSingleton<IShikimoriClient>(new ShikimoriClientWrapper(client));

        builder.Services.AddSingleton(RestService.For<IShikimoriApi>("https://shikimori.one/api"));
    }
}