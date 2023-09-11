using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.Services;
using AspNetCore.Extensions.AppModules.ModuleTypes;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Modules.Common;

public class CommonModule : AppModule
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

        builder.Services.Scan(scan =>
            scan
                .FromAssemblyOf<IUserService>()
                .AddClasses(classes => 
                    classes.Where(x => x.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );
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