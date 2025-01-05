using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace DAL.Contexts
{
    public class FavoriteDbContext : DbContext
    {
        public FavoriteDbContext(DbContextOptions<FavoriteDbContext> options) : base(options) { }
        public DbSet<GitHubRepository> Favorite { get; set; }
        public DbSet<Favorite> favorite { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GitHubOwner>()
                .HasNoKey();  
        }
        //public DbSet<GitHubRepository> Repositories { get; set; }

    }
}
