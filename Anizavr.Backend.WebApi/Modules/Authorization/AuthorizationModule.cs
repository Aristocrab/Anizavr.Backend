using System.Text;
using Anizavr.Backend.WebApi.Configuration;
using AspNetCore.Extensions.AppModules.ModuleTypes;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Modules.Authorization;

public class AuthorizationModule : AppModule
{
    private readonly IWebApiConfiguration _configuration;

    public AuthorizationModule(IWebApiConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(
            config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = _configuration.JwtAudience,
                    ValidIssuer = _configuration.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration.JwtSecretKey)
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