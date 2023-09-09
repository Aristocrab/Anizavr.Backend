namespace Anizavr.Backend.WebApi.Configuration;

public interface IWebApiConfiguration
{
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public string JwtSecretKey { get; set; }
    public string DatabasePath { get; set; }
    public string ConnectionString { get; set; }
    public string ShikimoriClientName { get; set; }
    public string ShikimoriClientId { get; set; }
    public string ShikimoriClientKey { get; set; }
    public string KodikKey { get; set; }
}