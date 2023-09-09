using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.Database;
using Anizavr.Backend.WebApi.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IWebApiConfiguration>();
        
        Directory.CreateDirectory(configuration.DatabasePath);
        builder.Services.AddDbContext<AnizavrDbContext>(options =>
            options.UseSqlite(configuration.ConnectionString));

        builder.Services.AddScoped<IAnizavrDbContext>(services => 
            services.GetRequiredService<AnizavrDbContext>());
    }
}