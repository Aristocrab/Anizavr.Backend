namespace Anizavr.Backend.Application.Dtos;

public class CommentAuthorDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string AvatarUrl { get; set; }
}