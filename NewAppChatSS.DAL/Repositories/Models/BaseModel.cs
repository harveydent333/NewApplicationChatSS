using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    public abstract class BaseModel<TEntity, TKey, TContext> : IBaseModel<TEntity, TKey, TContext>
        where TContext : DbContext
        where TEntity : EntityBase<TKey>, new()
    {
        /// <summary>
        /// Список идентификаторов сущностей
        /// </summary>
        public TKey[] Ids { get; set; }

        public virtual IQueryable<TEntity> GetQuarable(TContext context)
        {
            var query = (IQueryable<TEntity>)context.Set<TEntity>();

            if (Ids?.Length > 0)
            {
                if (Ids.Length == 1)
                {
                    var id = Ids[0];
                    query = query.Where(m => Equals(m.Id, id));
                }
                else
                {
                    query = query.Where(m => Ids.Contains(m.Id));
                }
            }

            return query;
        }
    }
}
