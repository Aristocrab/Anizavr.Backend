using Anizavr.Backend.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anizavr.Backend.Application.Database;

public sealed class AnizavrDbContext : DbContext, IAnizavrDbContext
{
    public AnizavrDbContext(DbContextOptions<AnizavrDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public required DbSet<User> Users { get; set; }
    public required DbSet<Comment> Comments { get; set; }
    public required DbSet<UserWatchingAnime> UserWatchingAnimeList { get; set; }
    public required DbSet<UserWatchedAnime> UserWatchedAnimeList { get; set; }
    public required DbSet<WishlistAnime> Wishlist { get; set; }
    public required DbSet<TierlistAnime> Tierlist { get; set; }
}