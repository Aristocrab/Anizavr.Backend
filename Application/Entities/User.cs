using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    [Column(TypeName = "TEXT")]
    public required Email Email { get; set; }
    public required string Password { get; set; }
    public required string AvatarUrl { get; set; }

    public required List<UserWatchingAnime> CurrentlyWatchingAnime { get; set; }
    public required List<UserWatchedAnime> WatchedAnime { get; set; }
}