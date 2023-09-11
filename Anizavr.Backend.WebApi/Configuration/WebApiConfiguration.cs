namespace Anizavr.Backend.WebApi.Configuration;

public class WebApiConfiguration : IWebApiConfiguration
{
    public required string JwtIssuer { get; init; }
    public required string JwtAudience { get; init; }
    public required string JwtSecretKey { get; init; }
    public required string DatabasePath { get; init; }
    public required string ConnectionString { get; init; }
    public required string ShikimoriClientName { get; init; }
    public required string ShikimoriClientId { get; init; }
    public required string ShikimoriClientKey { get; init; }
    public required string KodikKey { get; init; }
}