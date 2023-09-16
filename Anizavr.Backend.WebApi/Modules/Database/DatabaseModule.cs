using Anizavr.Backend.Application.Interfaces;
using Anizavr.Backend.Persistence.Database;
using Anizavr.Backend.WebApi.Configuration;
using AspNetCore.Extensions.AppModules.ModuleTypes;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    private readonly IWebApiConfiguration _configuration;

    public DatabaseModule(IWebApiConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Directory.CreateDirectory(_configuration.DatabasePath);
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(_configuration.ConnectionString));
        
        builder.Services.AddScoped<IAppDbContext>(services => 
            services.GetRequiredService<AppDbContext>());
    }
}