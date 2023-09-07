using Anizavr.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.Application.Database;

public sealed class AnizavrDbContext : DbContext, IAnizavrDbContext
{
    public AnizavrDbContext(DbContextOptions<AnizavrDbContext> options) : base(options)
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