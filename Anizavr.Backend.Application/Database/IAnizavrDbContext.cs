using Anizavr.Backend.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.Application.Database;

public interface IAnizavrDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserWatchingAnime> UserWatchingAnimeList { get; set; }
    public DbSet<UserWatchedAnime> UserWatchedAnimeList { get; set; }
    public DbSet<WishlistAnime> Wishlist { get; set; }
    public DbSet<TierlistAnime> Tierlist { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}