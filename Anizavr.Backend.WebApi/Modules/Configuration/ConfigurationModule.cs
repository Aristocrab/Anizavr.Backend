using Anizavr.Backend.Application.Configuration;
using Anizavr.Backend.WebApi.Configuration;
using Aristocrab.AppModules;

namespace Anizavr.Backend.WebApi.Modules.Configuration;

public class ConfigurationModule : AppModule
{
    public override int OrderIndex => -1;

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables("ANIZAVR_");
        var config = builder.Configuration;
        
        var appConfiguration = new ApplicationConfiguration
        {
            ShikimoriClientId = config.GetRequiredValue("ShikimoriClient:Id"),
            ShikimoriClientName = config.GetRequiredValue("ShikimoriClient:Name"),
            ShikimoriClientKey = config.GetRequiredValue("ANIZAVR_ShikimoriClientKey"),
            AnimeSkipKey = config.GetRequiredValue("ANIZAVR_AnimeSkipKey"),
            KodikKey = config.GetRequiredValue("ANIZAVR_KodikKey")
        };

        var webApiConfiguration = new WebApiConfiguration
        {
            JwtIssuer = config.GetRequiredValue("Jwt:Issuer"),
            JwtAudience = config.GetRequiredValue("Jwt:Audience"),
            JwtSecretKey = config.GetRequiredValue("ANIZAVR_JwtSecretKey"),
            ConnectionString = config.GetRequiredValue("Database:ConnectionString"),
            DatabasePath = config.GetRequiredValue("Database:Path"),
        };
        
        builder.Services.AddSingleton<IApplicationConfiguration>(appConfiguration);
        builder.Services.AddSingleton<IWebApiConfiguration>(webApiConfiguration);
    }
}