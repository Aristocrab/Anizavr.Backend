namespace Anizavr.Backend.Application.KodikApi.Entities;

public class KodikResults
{
    public required string Time { get; set; }
    public required int Total { get; set; }
    public required List<Result> Results { get; set; }
}