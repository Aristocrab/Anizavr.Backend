namespace Anizavr.Backend.Application.Entities;

public class UserWatchingAnime {
    public Guid Id { get; set; }
    public required long AnimeId { get; set; }
    public required int CurrentEpisode { get; set; }
    public required int EpisodesTotal { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string Rating { get; set; }
    public required int SecondsWatched { get; set; }
    public required float SecondsTotal { get; set; }
    public required string Kind { get; set; }
}