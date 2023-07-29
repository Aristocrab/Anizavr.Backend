namespace Anizavr.Backend.Application.AnimeSkipApi.Entities;

public class EpisodeTimestamps
{
    public Guid Id { get; set; }
    public required long AnimeId { get; set; }
    public required int Episode { get; set; }
    public required double? OpeningStart { get; set; }
    public required double? OpeningEnd { get; set; }
    public required double? EndingStart { get; set; }
}