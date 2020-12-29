using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public abstract class BaseRepository<TEntity, TKey, TContext, TModel> : IBaseRepository<TEntity, TKey, TContext, TModel>
        where TContext : DbContext
        where TModel : IBaseModel<TEntity, TKey, TContext>, new()
        where TEntity : EntityBase<TKey>, new()
    {
        protected BaseRepository(TContext context)
        {
            Context = context ?? throw new NullReferenceException();
            EntityOriginal = context.Set<TEntity>();
        }

        protected DbSet<TEntity> EntityOriginal { get; }

        protected virtual IQueryable<TEntity> Entity => EntityOriginal;

        protected TContext Context { get; }

        public async virtual Task<List<TEntity>> GetAsync(TModel model)
        {
            var query = model.GetQuarable(Context);
            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async virtual Task<TEntity> GetFirstOrDefaultAsync(TModel model)
        {
            var query = model.GetQuarable(Context);
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async virtual Task<TEntity> GetLastOrDefaultAsync(TModel model)
        {
            var query = model.GetQuarable(Context);
            return await query.OrderByDescending(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async virtual Task<long> GetCountAsync(TModel model)
        {
            var query = model.GetQuarable(Context);
            return await query.CountAsync().ConfigureAwait(false);
        }

        public async virtual Task<bool> IsExistAsync(TModel model)
        {
            var query = model.GetQuarable(Context);
            return await query.AnyAsync().ConfigureAwait(false);
        }

        public async virtual Task AddAsync(TEntity entity)
        {
            await EntityOriginal.AddAsync(entity).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async virtual Task ModifyAsync(TEntity entity)
        {
            EntityOriginal.Update(entity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async virtual Task DeleteAsync(TEntity entity)
        {
            EntityOriginal.Remove(entity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
