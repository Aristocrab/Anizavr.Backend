using Anizavr.Backend.Application.Services;
using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Definitions.Common;

public class CommonDefinition: AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        builder.Services.AddResponseCaching();
        builder.Services.AddControllers(options =>
        {
            options.CacheProfiles.Add("DefaultCacheProfile",
                new CacheProfile
                {
                    Duration = 86400,
                    Location = ResponseCacheLocation.Any,
                    VaryByQueryKeys = new []{ "*" }
                });
        });

        builder.Services.AddScoped<AnimeService>();
        builder.Services.AddScoped<UserService>();
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyOrigin();
            policyBuilder.AllowAnyMethod();
        });
        
        app.UseResponseCaching();

        app.MapGet("/", () => Results.Redirect("/swagger/index.html", true));
        app.MapHealthChecks("/health");
        app.MapControllers();
    }
}