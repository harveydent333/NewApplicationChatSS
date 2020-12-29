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
        Task<List<TEntity>> GetAsync(TModel model);

        Task<TEntity> GetFirstOrDefaultAsync(TModel model);

        Task<TEntity> GetLastOrDefaultAsync(TModel model);

        Task<long> GetCountAsync(TModel model);

        Task<bool> IsExistAsync(TModel model);

        Task AddAsync(TEntity entity);

        Task ModifyAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}
