using AspNetCore.Extensions.AppModules.ModuleTypes;

namespace AspNetCore.Extensions.AppModules;

public class AppModulesCollection
{
    public List<AppModule> AppModules { get; } = new();
}