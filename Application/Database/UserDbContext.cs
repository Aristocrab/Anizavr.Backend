using Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    
    public required DbSet<User> Users { get; set; }
    public required DbSet<Comment> Comments { get; set; }
    public required DbSet<UserWatchingAnime> UserWatchingAnimeList { get; set; }
    public required DbSet<UserWatchedAnime> UserWatchedAnimeList { get; set; }
    public required DbSet<WishlistAnime> Wishlist { get; set; }
}