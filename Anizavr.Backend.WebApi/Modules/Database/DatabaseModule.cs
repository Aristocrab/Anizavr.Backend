using Aristocrab.AppModules;
using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Shared;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Directory.CreateDirectory(Constants.DatabasePath);
        builder.Services.AddDbContext<AnizavrDbContext>(options =>
            options.UseSqlite(Constants.ConnectionString));

        builder.Services.AddScoped<IAnizavrDbContext>(services => services.GetRequiredService<AnizavrDbContext>());
    }
}