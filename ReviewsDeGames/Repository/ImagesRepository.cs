using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Repository
{
    public interface IImagesRepository
    {
        public Task Create(Image image);
        public Task Update(int id, Image image);
        public Task Delete(int id);
        public ValueTask<Image?> GetById(int id);
        public IQueryable<Image> GetQuery();
    }

    public class ImagesRepository : RepositoryBase<Image, int>, IImagesRepository
    {
        protected ImagesRepository(ReviewGamesContext context, IDescribesService describes) : base(context, describes)
        {
        }
    }
}
