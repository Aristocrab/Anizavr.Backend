namespace Anizavr.Backend.WebApi.Configuration;

public interface IWebApiConfiguration
{
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public string JwtSecretKey { get; }
    public string DatabasePath { get; }
    public string ConnectionString { get; }
    public string ShikimoriClientName { get; }
    public string ShikimoriClientId { get; }
    public string ShikimoriClientKey { get; }
    public string KodikKey { get; }
}