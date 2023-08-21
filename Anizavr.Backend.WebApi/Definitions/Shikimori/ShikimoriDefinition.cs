using Anizavr.Backend.Application.Shared;
using Anizavr.Backend.Application.ShikimoriApi;
using Calabonga.AspNetCore.AppDefinitions;
using NSubstitute;
using Refit;
using ShikimoriSharp;
using ShikimoriSharp.Bases;

namespace Anizavr.Backend.WebApi.Definitions.Shikimori;

public class ShikimoriDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(Constants.ShikimoriClientName, 
            Constants.ShikimoriClientId, Constants.ShikimoriClientKey);
        builder.Services.AddSingleton(RestService.For<IShikimoriApi>("https://shikimori.one/api"));
        builder.Services.AddSingleton(new ShikimoriClient(logger, settings));
    }
}