using Anizavr.Backend.Application.KodikApi;
using Calabonga.AspNetCore.AppDefinitions;
using Refit;

namespace Anizavr.Backend.WebApi.Definitions.Kodik;

public class KodikDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(RestService.For<IKodikApi>("https://kodikapi.com"));
    }
}