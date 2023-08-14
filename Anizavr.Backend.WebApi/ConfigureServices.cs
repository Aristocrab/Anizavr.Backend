using System.Globalization;
using System.Text;
using Anizavr.Backend.Application.AnimeSkipApi;
using Anizavr.Backend.Application.Database;
using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.Application.Services;
using Anizavr.Backend.Application.Shared;
using Anizavr.Backend.Application.ShikimoriApi;
using Anizavr.Backend.Application.Validators;
using FluentValidation;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Refit;
using Serilog;
using Serilog.Events;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Anizavr.Backend.WebApi;

public static class ConfigureServices
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        // Main
        builder.Services.AddResponseCaching();
        builder.Services.AddHealthChecks();
        builder.Services.AddControllers(options =>
        {
            options.CacheProfiles.Add("DefaultCacheProfile",
                new CacheProfile
                {
                    Duration = 86400,
                    Location = ResponseCacheLocation.Any,
                    VaryByQueryKeys = new []{ "*" }
                });
        });
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        // Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console()
            .CreateLogger();
        builder.Host.UseSerilog();
        
        // FluentValidation
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ru");
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidatior>();

        // Shikimori
        var logger = Substitute.For<ILogger>();
        var settings = new ClientSettings(Constants.ShikimoriClientName, 
            Constants.ShikimoriClientId, Constants.ShikimoriClientKey);
        builder.Services.AddSingleton(RestService.For<IShikimoriApi>("https://shikimori.one/api"));
        builder.Services.AddSingleton(new ShikimoriClient(logger, settings));
        
        // Kodik
        builder.Services.AddSingleton(RestService.For<IKodikApi>("https://kodikapi.com"));
        
        // Anime-skip
        builder.Services.AddSingleton<IGraphQLClient>(new GraphQLHttpClient("https://api.anime-skip.com/graphql", 
            new SystemTextJsonSerializer()));
        builder.Services.AddScoped<AnimeSkipService>();

        // Database
        Directory.CreateDirectory(Constants.DatabasePath);
        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite(Constants.ConnectionString));

        // Services
        builder.Services.AddScoped<AnimeService>();
        builder.Services.AddScoped<UserService>();
        
        // JWT Auth
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

        // Swagger
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
        });
    }
}