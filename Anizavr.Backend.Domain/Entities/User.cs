using Anizavr.Backend.Domain.Common;

namespace Anizavr.Backend.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }
    public required string AvatarUrl { get; set; }
    public required List<UserWatchingAnime> CurrentlyWatchingAnime { get; set; }
    public required List<UserWatchedAnime> WatchedAnime { get; set; }
    public required List<WishlistAnime> Wishlist { get; set; }
    public required List<TierlistAnime> Tierlist { get; set; }
}