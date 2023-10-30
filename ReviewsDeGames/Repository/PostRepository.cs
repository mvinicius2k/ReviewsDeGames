using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Repository
{
    public interface IPostRepository
    {
        public IQueryable<Post> GetQuery();
        public ValueTask<Post?> GetById(int id);
        public Task Create(Post post);
        public Task Update(int id, Post post);
        public Task Delete(int id);


    }

    public class PostRepository : RepositoryBase<Post, int>, IPostRepository
    {
        public PostRepository(ReviewGamesContext context, IDescribesService describes) : base(context, describes)
        {
        }
    }
}
