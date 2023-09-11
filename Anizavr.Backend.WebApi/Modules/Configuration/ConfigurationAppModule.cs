using Anizavr.Backend.WebApi.Configuration;
using AspNetCore.Extensions.AppModules;
using AspNetCore.Extensions.AppModules.ModuleTypes;

namespace Anizavr.Backend.WebApi.Modules.Configuration;

public class ConfigurationModule : ConfigurationAppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables("ANIZAVR_");
        var config = builder.Configuration;

        var webApiConfiguration = new WebApiConfiguration
        {
            JwtIssuer = config.GetRequiredValue("Jwt:Issuer"),
            JwtAudience = config.GetRequiredValue("Jwt:Audience"),
            JwtSecretKey = config.GetRequiredValue("ANIZAVR_JwtSecretKey"),
            ConnectionString = config.GetRequiredValue("Database:ConnectionString"),
            DatabasePath = config.GetRequiredValue("Database:Path"),
            ShikimoriClientId = config.GetRequiredValue("ShikimoriClient:Id"),
            ShikimoriClientName = config.GetRequiredValue("ShikimoriClient:Name"),
            ShikimoriClientKey = config.GetRequiredValue("ANIZAVR_ShikimoriClientKey"),
            KodikKey = config.GetRequiredValue("ANIZAVR_KodikKey")
        };
        
        builder.Services.AddSingleton<IWebApiConfiguration>(webApiConfiguration);
    }
}