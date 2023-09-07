namespace Anizavr.Backend.Domain.Entities.Kodik;

public class KodikResults
{
    public required string Time { get; set; }
    public required int Total { get; set; }
    public required List<Result> Results { get; set; }
}