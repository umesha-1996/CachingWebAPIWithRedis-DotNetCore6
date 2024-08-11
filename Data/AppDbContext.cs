using CachingWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingWebAPI.Data;

public class AppDbContext : DbContext
{
    public DbSet<Drivers> Drivers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}
