﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;
using System.Runtime.CompilerServices;

namespace ReviewsDeGames.Repository
{
    public class RepositoryBase<TModel, TId> where TModel : class,IModel<TId>
    {
        protected readonly ReviewGamesContext _context;
        protected readonly IDescribesService _describes;

        protected RepositoryBase(ReviewGamesContext context, IDescribesService describes)
        {
            _context = context;
            _describes = describes;
        }

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
        /// <inheritdoc/>
        public virtual Task Delete(TId id)
        {
            var model = _context.Set<TModel>().Find(ToArray(id));
            if (model == null)
                throw new KeyNotFoundException(_describes.KeyNotFound(id));
            _context.Set<TModel>().Remove(model);
            return _context.SaveChangesAsync();
        }
        /// <inheritdoc/>
        public virtual IQueryable<TModel> GetQuery()
        {
            return _context.Set<TModel>().AsQueryable();
        }
        /// <inheritdoc/>
        public virtual ValueTask<TModel?> GetById(TId id)
        {

            return _context.Set<TModel>().FindAsync(ToArray(id));
        }
        /// <inheritdoc/>
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
