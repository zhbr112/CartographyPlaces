using Microsoft.EntityFrameworkCore;
using CartographyPlaces.AuthAPI.Models;

namespace CartographyPlaces.AuthAPI.Data;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}
