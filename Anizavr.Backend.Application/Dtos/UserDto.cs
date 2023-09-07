using Anizavr.Backend.Domain.Entities;

namespace Anizavr.Backend.Application.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string AvatarUrl { get; set; }

    public required List<UserWatchingAnime> CurrentlyWatchingAnime { get; set; }
    public required List<UserWatchedAnime> WatchedAnime { get; set; }
    public required List<WishlistAnime> Wishlist { get; set; }
    public required List<TierlistAnime> Tierlist { get; set; }
}