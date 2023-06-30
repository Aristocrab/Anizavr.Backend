namespace Application.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string AvatarUrl { get; set; }

    public required List<UserAnime> LastWatchedAnimes { get; set; }
}

public class UserAnime {
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