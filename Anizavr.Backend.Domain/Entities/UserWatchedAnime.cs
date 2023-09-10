using Anizavr.Backend.Domain.Common;

namespace Anizavr.Backend.Domain.Entities;

public class UserWatchedAnime : BaseEntity
{
    public required long AnimeId { get; set; }
    public required int CurrentEpisode { get; set; }
    public required int EpisodesTotal { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string Rating { get; set; }
    public required int? UserScore { get; set; }
}