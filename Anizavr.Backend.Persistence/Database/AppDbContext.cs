using Anizavr.Backend.Application.Interfaces;
using Anizavr.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.Persistence.Database;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<UserWatchingAnime> UserWatchingAnimeList { get; set; } = null!;
    public DbSet<UserWatchedAnime> UserWatchedAnimeList { get; set; } = null!;
    public DbSet<WishlistAnime> Wishlist { get; set; } = null!;
    public DbSet<TierlistAnime> Tierlist { get; set; } = null!;
}