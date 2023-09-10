namespace Anizavr.Backend.Application.Dtos;

public class AddCommentDto
{
    public required long AnimeId { get; init; }
    public required string Text { get; init; }
}