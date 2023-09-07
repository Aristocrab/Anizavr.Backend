using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.ShikimoriApi;
using NSubstitute;
using Refit;
using ShikimoriSharp;
using ShikimoriSharp.Bases;

namespace Anizavr.Backend.WebApi.Modules.Shikimori;

public class ShikimoriModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(configuration["ShikimoriClient:Name"]!, 
            configuration["ShikimoriClient:Id"], configuration["ANIZAVR_ShikimoriClientKey"]!);
        var client = new ShikimoriClient(logger, settings);
        builder.Services.AddSingleton<IShikimoriClient>(new ShikimoriClientWrapper(client));

        builder.Services.AddSingleton(RestService.For<IShikimoriApi>("https://shikimori.one/api"));
    }
}