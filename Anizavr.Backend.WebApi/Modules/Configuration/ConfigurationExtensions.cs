namespace Anizavr.Backend.WebApi.Modules.Configuration;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name)
    {
        var value = configuration[name];
        if (value is null)
        {
            throw new Exception($"Configuration value {name} is not set");
        }

        return value;
    }
}