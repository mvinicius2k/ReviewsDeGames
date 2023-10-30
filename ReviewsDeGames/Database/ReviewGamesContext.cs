using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Models;

namespace ReviewsDeGames.Database
{
    public class ReviewGamesContext : IdentityDbContext<User>
    {

        public DbSet<Image> Images { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }
        public ReviewGamesContext(DbContextOptions<ReviewGamesContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Image>()
                .HasOne(i => i.Owner)
                .WithMany(i => i.Images);
            builder.Entity<Post>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Posts)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserVote>()
                .HasOne(uv => uv.Post)
                .WithMany(p => p.Votes)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserVote>()
                .HasOne(uv => uv.User)
                .WithMany(u => u.Votes)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Image>().HasIndex(i => i.FileName).IsUnique();
            builder.Entity<UserVote>().HasKey(uv => new { uv.UserId, uv.PostId });
        }
    }
}
