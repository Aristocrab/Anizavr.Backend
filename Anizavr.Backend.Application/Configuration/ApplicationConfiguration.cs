namespace Anizavr.Backend.Application.Configuration;

public class ApplicationConfiguration : IApplicationConfiguration
{
    public required string ShikimoriClientName { get; set; }
    public required string ShikimoriClientId { get; set; }
    public required string ShikimoriClientKey { get; set; }
    public required string AnimeSkipKey { get; set; }
    public required string KodikKey { get; set; }
}