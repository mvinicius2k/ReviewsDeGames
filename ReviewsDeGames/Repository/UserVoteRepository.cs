using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Repository
{

    public class UserVoteRepository : RepositoryBase<UserVote, (string user, int post)>, IUserVoteRepository
    {
        public UserVoteRepository(ReviewGamesContext context, IDescribesService describes) : base(context, describes)
        {
        }
    }
}
