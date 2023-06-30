using Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Database;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    
    public required DbSet<User> Users { get; set; }
    public required DbSet<UserAnime> UserAnimes { get; set; }
}