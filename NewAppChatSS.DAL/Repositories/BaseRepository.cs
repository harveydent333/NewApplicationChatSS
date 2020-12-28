using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async virtual Task<long> GetCountAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<TEntity> GetFirstOrDefaultAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<TEntity> GetLastOrDefaultAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<bool> IsExistAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        public async virtual Task AddAsync(IEnumerable<TEntity> rows)
        {
            throw new NotImplementedException();
        }

        public async virtual Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async virtual Task ModifyAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async virtual Task ModifyAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async virtual Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async virtual Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            EntityOriginal.RemoveRange(entities);
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}/*

    protected string TableName => typeof(TEntity).Name;

    protected DbSet<TEntity> EntityOriginal { get; }

    protected virtual IQueryable<TEntity> Entity => EntityOriginal;

    protected TContext Context { get; }

    public virtual async Task<TEntity> GetFirstOrDefaultAsync(TModel model)
    {
        var query = model.GetQuarable(Context);
        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public virtual async Task<TEntity> GetLastOrDefaultAsync(TModel model)
    {
        var query = model.GetQuarable(Context);
        return await query.OrderByDescending(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public virtual async Task<long> GetCountAsync(TModel model)
    {
        var query = model.GetQuarable(Context);
        return await query.CountAsync().ConfigureAwait(false);
    }

    public virtual async Task<List<TEntity>> GetAsync(TExtendedModel model)
    {
        var query = model.GetQuarable(Context);
        return await query.ToListAsync().ConfigureAwait(false);
    }

    public virtual async Task<bool> IsExistAsync(TModel model)
    {
        var query = model.GetQuarable(Context);
        return await query.AnyAsync().ConfigureAwait(false);
    }

    public virtual async Task AddAsync(TEntity row)
    {
        row.CreatedAt = DateTime.Now;
        row.UpdatedAt = row.CreatedAt;

        await EntityOriginal.AddAsync(row).ConfigureAwait(false);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task AddAsync(IEnumerable<TEntity> rows)
    {
        foreach (var row in rows)
        {
            row.CreatedAt = DateTime.Now;
            row.UpdatedAt = row.CreatedAt;
        }

        await EntityOriginal.AddRangeAsync(rows).ConfigureAwait(false);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task ModifyAsync(TEntity row)
    {
        row.UpdatedAt = DateTime.Now;

        // NOTE: Внедрена единая логика работы с изменениям между методами ModifyAsync, ModifyAsync
        EntityOriginal.Update(row);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task ModifyAsync(IEnumerable<TEntity> rows)
    {
        foreach (var row in rows)
        {
            row.UpdatedAt = DateTime.Now;
        }

        EntityOriginal.UpdateRange(rows);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(TModel model)
    {
        var query = model.GetQuarable(Context);
        EntityOriginal.RemoveRange(query);

        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        EntityOriginal.Remove(entity);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
    {
        EntityOriginal.RemoveRange(entities);
        await Context.SaveChangesAsync().ConfigureAwait(false);
    }
*/