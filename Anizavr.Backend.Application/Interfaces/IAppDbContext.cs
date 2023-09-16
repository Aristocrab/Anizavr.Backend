using Anizavr.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Comment> Comments { get; }
    public DbSet<UserWatchingAnime> UserWatchingAnimeList { get; }
    public DbSet<WishlistAnime> Wishlist { get; }
    public DbSet<TierlistAnime> Tierlist { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}