namespace Application.KodikApi.Entities;

public class KodikAnime
{
    public required string Time { get; set; }
    public required int Total { get; set; }
    public required List<Result> Results { get; set; }
}