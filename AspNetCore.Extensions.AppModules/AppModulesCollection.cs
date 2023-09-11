namespace AspNetCore.Extensions.AppModules;

public class AppModulesCollection
{
    public List<IAppModule> AppModules { get; } = new();
    public List<IConfigurationModule> ConfigurationModules { get; } = new();
}