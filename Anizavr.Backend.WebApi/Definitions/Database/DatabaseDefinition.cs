using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.Shared;
using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.WebApi.Definitions.Database;

public class DatabaseDefinition: AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Directory.CreateDirectory(Constants.DatabasePath);
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite(Constants.ConnectionString));
    }
}