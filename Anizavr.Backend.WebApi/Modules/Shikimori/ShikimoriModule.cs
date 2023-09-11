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
    private readonly IWebApiConfiguration _configuration;

    public ShikimoriModule(IWebApiConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(
            _configuration.ShikimoriClientName, 
            _configuration.ShikimoriClientId, 
            _configuration.ShikimoriClientKey);
        
        var client = new ShikimoriClient(logger, settings);
        var shikimoriRestService = RestService.For<IShikimoriApi>("https://shikimori.one/api");
        
        builder.Services.AddSingleton<IShikimoriClient>(new ShikimoriClientAdapter(client, shikimoriRestService));
    }
}