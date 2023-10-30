using ReviewsDeGames.Models;

namespace ReviewsDeGames.Repository
{
    public interface IUserVoteRepository
    {
        public IQueryable<UserVote> GetQuery();
        public ValueTask<UserVote?> GetById((string user, int post) id);
        public Task Create(UserVote userVote);
        public Task Update((string user, int post) id, UserVote userVote);
        public Task Delete((string user, int post) id);
    }
}
