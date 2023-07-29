namespace Anizavr.Backend.Application.Dtos;

public class AddCommentDto
{
    public required long AnimeId { get; set; }
    public required string Text { get; set; }
}