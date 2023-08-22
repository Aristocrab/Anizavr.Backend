using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Shared;
using Aristocrab.AspNetCore.AppModules;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Directory.CreateDirectory(Constants.DatabasePath);
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite(Constants.ConnectionString));
    }
}