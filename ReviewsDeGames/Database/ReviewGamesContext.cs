using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Models;

namespace ReviewsDeGames.Database
{
    public class ReviewGamesContext : IdentityDbContext<User>
    {

        public DbSet<Image> Images { get; set; }

        public ReviewGamesContext(DbContextOptions<ReviewGamesContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Image>()
                .HasOne(i => i.Owner)
                .WithMany(i => i.Images);

            builder.Entity<Image>().HasIndex(i => i.FileName).IsUnique();
        }
    }
}
