using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IBaseRepository<TEntity, in TKey, TContext, TModel>
        where TContext : DbContext
        where TModel : IBaseModel<TEntity, TKey, TContext>, new()
        where TEntity : IEntityBase<TKey>, new()
    {
        Task<TEntity> GetFirstOrDefaultAsync(TModel model);

        Task<TEntity> GetLastOrDefaultAsync(TModel model);

        Task<long> GetCountAsync(TModel model);

        Task<List<TEntity>> GetAsync(TModel model);

        Task<bool> IsExistAsync(TModel model);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(IEnumerable<TEntity> entities);

        Task ModifyAsync(TEntity entity);

        Task ModifyAsync(IEnumerable<TEntity> entities);

        Task AddAsync(IEnumerable<TEntity> entities);

        Task AddAsync(TEntity entity);
    }
}
