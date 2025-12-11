using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimHistory> ClaimHistories { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Mappings.ClaimConfiguration());
            modelBuilder.ApplyConfiguration(new Mappings.ClaimHistoryConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
