using Anizavr.Backend.Domain.Common;

namespace Anizavr.Backend.Domain.Entities;

public class TierlistAnime : BaseEntity
{
    public int Position { get; set; }
    public required long AnimeId { get; set; }
    public required int EpisodesTotal { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string Rating { get; set; }
}