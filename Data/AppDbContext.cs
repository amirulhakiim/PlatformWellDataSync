using Microsoft.EntityFrameworkCore;
using PlatformWellDataSync.Models;

namespace PlatformWellDataSync.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Well> Wells { get; set; }
    }
}
