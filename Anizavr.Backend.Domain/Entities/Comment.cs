using Anizavr.Backend.Domain.Common;

namespace Anizavr.Backend.Domain.Entities;

public class Comment : BaseEntity
{
    public required User Author { get; set; }
    public required long AnimeId { get; set; }
    public required string Text { get; set; }
    public required DateTime Created { get; set; }
}