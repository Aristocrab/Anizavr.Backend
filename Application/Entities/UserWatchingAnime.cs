namespace Application.Entities;

public class UserWatchingAnime {
    public Guid Id { get; set; }
    public required long AnimeId { get; set; }
    public required int EpisodesWatched { get; set; }
    public required int NextEpisode { get; set; }
    public required int EpisodesTotal { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string Rating { get; set; }
    public required int SecondsWatched { get; set; }
}