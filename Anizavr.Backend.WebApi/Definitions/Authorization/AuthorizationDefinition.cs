using System.Text;
using Anizavr.Backend.Application.Shared;
using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.IdentityModel.Tokens;

namespace Anizavr.Backend.WebApi.Definitions.Authorization;

public class AuthorizationDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(
            config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = Constants.Audience,
                    ValidIssuer = Constants.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtSecretKey)),
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