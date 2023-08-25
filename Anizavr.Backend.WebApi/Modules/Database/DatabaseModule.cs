using Aristocrab.AppModules;
using Anizavr.Backend.Application.Database;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        Directory.CreateDirectory(configuration["Database:Path"]!);
        builder.Services.AddDbContext<AnizavrDbContext>(options =>
            options.UseSqlite(configuration["Database:ConnectionString"]!));

        builder.Services.AddScoped<IAnizavrDbContext>(services => 
            services.GetRequiredService<AnizavrDbContext>());
    }
}