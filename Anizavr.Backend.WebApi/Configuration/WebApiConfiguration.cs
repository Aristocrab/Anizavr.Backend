namespace Anizavr.Backend.WebApi.Configuration;

public class WebApiConfiguration : IWebApiConfiguration
{
    public required string JwtIssuer { get; set; }
    public required string JwtAudience { get; set; }
    public required string JwtSecretKey { get; set; }
    public required string DatabasePath { get; set; }
    public required string ConnectionString { get; set; }
}