using System.Text;
using Anizavr.Backend.WebApi.Configuration;
using AspNetCore.Extensions.AppModules;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Modules.Authorization;

public class AuthorizationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Services.BuildServiceProvider().GetRequiredService<IWebApiConfiguration>();
        
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(
            config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration.JwtAudience,
                    ValidIssuer = configuration.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.JwtSecretKey)
                        ),
                    ValidateLifetime = false
                };
            });
        builder.Services.AddAuthorization();
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}