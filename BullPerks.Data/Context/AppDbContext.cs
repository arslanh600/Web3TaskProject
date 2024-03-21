using BullPerks.Data.Entity;
using Microsoft.EntityFrameworkCore;


namespace BullPerks.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TokenData> TokenData { get; set; }
    }
}
