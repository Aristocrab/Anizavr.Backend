namespace Anizavr.Backend.Application.Configuration;

public interface IApplicationConfiguration
{
    public string ShikimoriClientName { get; set; }
    public string ShikimoriClientId { get; set; }
    public string ShikimoriClientKey { get; set; }
    public string AnimeSkipKey { get; set; }
    public string KodikKey { get; set; }
}