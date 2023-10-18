using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Models;

namespace ReviewsDeGames.Database
{
    public class ReviewGamesContext : IdentityDbContext<User>
    {

        public ReviewGamesContext(DbContextOptions<ReviewGamesContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
