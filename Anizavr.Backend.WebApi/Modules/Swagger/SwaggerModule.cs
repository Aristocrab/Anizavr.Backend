using Aristocrab.AspNetCore.AppModules;
using Microsoft.OpenApi.Models;

namespace Anizavr.Backend.WebApi.Modules.Swagger;

public class SwaggerModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer",
                Description = "Please insert JWT token into field"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            var filePath = Path.Combine(AppContext.BaseDirectory, "Anizavr.Backend.WebApi.xml");
            c.IncludeXmlComments(filePath);
        });
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}