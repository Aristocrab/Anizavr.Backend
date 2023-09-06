namespace Anizavr.Backend.WebApi.Configuration;

public interface IWebApiConfiguration
{
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public string JwtSecretKey { get; set; }
    public string DatabasePath { get; set; }
    public string ConnectionString { get; set; }
}