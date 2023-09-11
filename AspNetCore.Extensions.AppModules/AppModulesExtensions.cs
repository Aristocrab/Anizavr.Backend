using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Extensions.AppModules
{
    public static class AppModulesExtensions
    {
        private static readonly Func<Type, bool> IsNonAbstractIAppModule =
            type => typeof(IAppModule).IsAssignableFrom(type) && !type.IsAbstract;

        private static readonly Func<Type, bool> IsNonAbstractIConfigurationModule =
            type => typeof(IConfigurationModule).IsAssignableFrom(type) && !type.IsAbstract;

        public static void AddModules(this WebApplicationBuilder builder)
        {
            AddModules(builder, Assembly.GetCallingAssembly());
        }

        public static void AddModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            AddConfigurationModules(builder, assemblies);
            AddAppModules(builder, assemblies);
        }

        private static void AddConfigurationModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var configurationModules = assembly.GetTypes().Where(IsNonAbstractIConfigurationModule);

                var configurationInstances = configurationModules
                    .Select(Activator.CreateInstance)
                    .Cast<IConfigurationModule>()
                    .ToList();

                foreach (var instance in configurationInstances)
                {
                    if (instance.Enabled)
                    {
                        instance.ConfigureServices(builder);
                    }
                }
            }
        }

        private static void AddAppModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            var modulesCollection = new AppModulesCollection();
            ILogger<AppModule>? logger = null;

            foreach (var assembly in assemblies)
            {
                var builderServiceProvider = builder.Services.BuildServiceProvider();

                var modules = assembly.GetTypes().Where(IsNonAbstractIAppModule);

                var instances = modules
                    .Select(x => ActivatorUtilities.CreateInstance(builderServiceProvider, x))
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
                logger.LogDebug("[{Assembly}] AppModules found: {AppModulesCount}",
                    assembly.GetName().Name, modulesCollection.AppModules.Count);

                foreach (var instance in instances.OrderBy(x => x.GetType().Name))
                {
                    logger.LogDebug("{Type} (Enabled: {Enabled})",
                        instance.GetType().Name, instance.Enabled);
                }
            }
            builder.Services.AddSingleton(modulesCollection);
        }

        public static void UseModules(this WebApplication app)
        {
            var modulesCollection = app.Services.GetRequiredService<AppModulesCollection>();

            foreach (var module in modulesCollection.AppModules)
            {
                if (module.Enabled)
                {
                    module.ConfigureApplication(app);
                }
            }

            var logger = app.Services
                .GetRequiredService<ILogger<AppModule>>();
            logger.LogDebug("AppModules ({Count}) configured", modulesCollection.AppModules.Count);
        }
    }
}
