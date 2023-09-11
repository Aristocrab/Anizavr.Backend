namespace Anizavr.Backend.WebApi.Modules.Configuration;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name)
    {
        var value = configuration[name];
        if (value is null or "")
        {
            throw new ArgumentException($"Configuration value {name} is not set", nameof(name));
        }

        return value;
    }
}