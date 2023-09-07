using System.Text;
using AspNetCore.Extensions.AppModules;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Modules.Authorization;

public class AuthorizationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(
            config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["ANIZAVR_JwtSecretKey"]!
                            )),
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