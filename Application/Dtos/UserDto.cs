using Application.Entities;

namespace Application.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string AvatarUrl { get; set; }

    public required List<UserWatchingAnime> LastWatchedAnimes { get; set; }
}