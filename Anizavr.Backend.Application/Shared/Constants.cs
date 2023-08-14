namespace Anizavr.Backend.Application.Shared;

public class Constants
{
    public const string Issuer = "https://anizavr.me";
    public const string Audience = "https://anizavr.me";
    public static readonly string JwtSecretKey = GetEnvironmentVariable("ANIZAVR_JwtSecretKey");

    public const string ShikimoriClientName = "AnimeWiki4All";
    public const string ShikimoriClientId = "jsnkSxyHQ4g5SiB9XXEBIF6akkHonGKNioQTsWEBxh4";
    public static readonly string ShikimoriClientKey = GetEnvironmentVariable("ANIZAVR_ShikimoriClientKey");
    
    public static readonly string AnimeSkipKey = GetEnvironmentVariable("ANIZAVR_AnimeSkipKey");
    
    public static readonly string KodikKey = GetEnvironmentVariable("ANIZAVR_KodikKey");
    public const string DatabasePath = "../Database/";
    public const string ConnectionString = $"Data Source={DatabasePath}anizavr.db";

    private static string GetEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User)
                    ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process)
                    ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);

        return value ?? throw new Exception($"Environment variable {name} is not set");
    }
}