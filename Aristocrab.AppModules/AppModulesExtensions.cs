using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aristocrab.AppModules;

public static class AppModulesExtensions
{
    #region AddModules

    public static void AddModules(this WebApplicationBuilder builder)
    {
        AddModules(builder, Assembly.GetCallingAssembly());
    }
    
    public static void AddModules(this WebApplicationBuilder builder, params Type[] types)
    {
        AddModules(builder, types.Select(x => x.Assembly).ToArray());
    }

    private static Func<Type, bool> AppModulePredicate => 
        type => typeof(AppModule).IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false };

    public static void AddModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        var modulesCollection = builder.Services.BuildServiceProvider().GetService<AppModulesCollection>() 
                                    ?? new AppModulesCollection();
        ILogger<AppModule>? logger = null;

        foreach (var assembly in assemblies)
        {
            var modules = assembly.GetTypes()
                .Where(AppModulePredicate);

            var instances = modules
                .Select(Activator.CreateInstance)
                .Cast<IAppModule>()
                .ToList();
        
            foreach (var instance in instances.OrderBy(x => x.OrderIndex))
            {
                if (instance.Enabled)
                {
                    instance.ConfigureServices(builder);
                }
                
                modulesCollection.AppModules.Add(instance);
            }

            logger ??= builder.Services
                .BuildServiceProvider()
                .GetRequiredService<ILogger<AppModule>>();
            logger.LogDebug("[{Assembly}] AppModules found: {Count}", assembly.GetName().Name, modulesCollection.AppModules.Count);

            foreach (var instance in instances.OrderBy(x => x.GetType().Name))
            {
                logger.LogDebug("{Type} (Enabled: {Enabled})", instance.GetType().Name, instance.Enabled);
            }
        }

        builder.Services.AddSingleton(modulesCollection);
    }

    #endregion
    
    #region UseModules

    public static void UseModules(this WebApplication app)
    {
        var modulesCollection = app.Services.GetRequiredService<AppModulesCollection>();

        foreach (var module in modulesCollection.AppModules)
        {
            module.ConfigureApplication(app);
        }
    }
    
    #endregion
}