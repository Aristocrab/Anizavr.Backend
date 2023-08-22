using System.Reflection;

namespace Anizavr.Backend.WebApi.Modules.Shared;

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
        x => typeof(AppModule).IsAssignableFrom(x)
             && x is
             {
                 IsAbstract: false,
                 IsInterface: false
             };

    public static void AddModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        var definitionsCollection = builder.Services.BuildServiceProvider().GetService<AppModulesCollection>() 
                                    ?? new AppModulesCollection();

        foreach (var assembly in assemblies)
        {
            var appDefinitions = assembly.GetTypes()
                .Where(AppModulePredicate);

            var instances = appDefinitions.Select(Activator.CreateInstance)
                .Cast<IAppModule>()
                .ToList();
        
            foreach (var instance in instances.OrderBy(x => x.OrderIndex))
            {
                if (instance.Enabled)
                {
                    instance.ConfigureServices(builder);
                }
                
                definitionsCollection.AppDefinitions.Add(instance);
            }

            var logger = builder.Services
                .BuildServiceProvider()
                .GetRequiredService<ILogger<AppModule>>();
            logger.LogDebug("[{Assembly}] AppModules found: {Count}", assembly.GetName().Name, definitionsCollection.AppDefinitions.Count);

            foreach (var instance in instances.OrderBy(x => x.GetType().Name))
            {
                logger.LogDebug("{Type} (Enabled: {Enabled})", instance.GetType().Name, instance.Enabled);
            }
        }

        builder.Services.AddSingleton(definitionsCollection);
    }

    #endregion
    
    #region UseModules

    public static void UseModules(this WebApplication app)
    {
        var definitionsCollection = app.Services.GetRequiredService<AppModulesCollection>();

        foreach (var appDefinition in definitionsCollection.AppDefinitions)
        {
            appDefinition.ConfigureApplication(app);
        }
    }
    
    #endregion
}