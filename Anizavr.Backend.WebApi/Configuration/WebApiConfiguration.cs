namespace Anizavr.Backend.WebApi.Configuration;

public class WebApiConfiguration : IWebApiConfiguration
{
    public required string JwtIssuer { get; set; }
    public required string JwtAudience { get; set; }
    public required string JwtSecretKey { get; set; }
    public required string DatabasePath { get; set; }
    public required string ConnectionString { get; set; }
    public required string ShikimoriClientName { get; set; }
    public required string ShikimoriClientId { get; set; }
    public required string ShikimoriClientKey { get; set; }
    public required string KodikKey { get; set; }
}