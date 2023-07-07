using Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    
    public required DbSet<User> Users { get; set; }
    public required DbSet<UserWatchingAnime> UserWatchingAnimeList { get; set; }
    public required DbSet<UserWatchedAnime> UserWatchedAnimeList { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .Property(e => e.Email)
            .HasConversion(
                convertToProviderExpression: email => email.Value,
                convertFromProviderExpression: email => Email.From(email));
    }
}