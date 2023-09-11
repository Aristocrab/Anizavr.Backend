using System.Reflection;
using AspNetCore.Extensions.AppModules.ModuleTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Extensions.AppModules
{
    public static class AppModulesExtensions
    {
        private static readonly Func<Type, bool> IsNonAbstractAppModule =
            type => typeof(AppModule).IsAssignableFrom(type) && !type.IsAbstract;

        private static readonly Func<Type, bool> IsNonAbstractConfigurationModule =
            type => typeof(ConfigurationAppModule).IsAssignableFrom(type) && !type.IsAbstract;

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
                var configurationModules = assembly.GetTypes().Where(IsNonAbstractConfigurationModule);

                var configurationInstances = configurationModules
                    .Select(Activator.CreateInstance)
                    .Cast<ConfigurationAppModule>()
                    .ToList();

                foreach (var instance in configurationInstances.OrderBy(x => x.OrderIndex))
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

                var modules = assembly.GetTypes().Where(IsNonAbstractAppModule);

                var instances = modules
                    .Select(x => ActivatorUtilities.CreateInstance(builderServiceProvider, x))
                    .Cast<AppModule>()
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
