using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;
using System.Runtime.CompilerServices;

namespace ReviewsDeGames.Repository
{
    /// <summary>
    /// Implementação comum para operações genéricas CRUD de repository
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class RepositoryBase<TModel, TId> where TModel : class,IModel<TId>
    {
        protected readonly ReviewGamesContext _context;
        protected readonly IDescribesService _describes;

        protected RepositoryBase(ReviewGamesContext context, IDescribesService describes)
        {
            _context = context;
            _describes = describes;
        }

        /// <summary>
        /// Converte um dado unitário ou tupla em um array de <see langword="object"></see>.
        /// Útil para usar em conjunto com o Find do EF Core genericamente
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private object[] ToArray(TId key)
        {
            if (key is ITuple)
            {
                var tuple = (ITuple)key;
                var result = new object[tuple.Length];
                for (int i = 0; i < tuple.Length; i++)
                {
                    result[i] = tuple[i];
                }
                return result;
            }
            else
            {
                return new object[] { key };
            }
        }


        public virtual Task Create(TModel model)
        {

            _context.Set<TModel>().Add(model);
            return _context.SaveChangesAsync();
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual Task Delete(TId id)
        {
            var model = _context.Set<TModel>().Find(ToArray(id));
            if (model == null)
                throw new KeyNotFoundException(_describes.KeyNotFound(id));
            _context.Set<TModel>().Remove(model);
            return _context.SaveChangesAsync();
        }
        
        public virtual IQueryable<TModel> GetQuery()
        {
            return _context.Set<TModel>().AsQueryable();
        }
        
        public virtual ValueTask<TModel?> GetById(TId id)
        {

            return _context.Set<TModel>().FindAsync(ToArray(id));
        }
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual Task Update(TId id, TModel model)
        {
            var entity = _context.Set<TModel>().Find(ToArray(id));
            if (entity == null)
                throw new KeyNotFoundException(_describes.KeyNotFound(id));

            _context.Set<TModel>().Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            model.SetId(id);

            _context.Set<TModel>().Update(model);
            return _context.SaveChangesAsync();
        }
    }
}
