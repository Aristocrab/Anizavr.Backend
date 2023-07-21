namespace Application.Dtos;

public class CommentDto
{
    public Guid Id { get; set; }
    public required CommentAuthorDto Author { get; set; }
    public required long AnimeId { get; set; }
    public required string Text { get; set; }
}