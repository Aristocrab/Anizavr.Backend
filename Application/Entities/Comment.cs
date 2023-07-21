namespace Application.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public required User Author { get; set; }
    public required long AnimeId { get; set; }
    public required string Text { get; set; }
    public required DateTime Created { get; set; }
}