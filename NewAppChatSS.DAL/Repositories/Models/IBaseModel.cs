using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    public interface IBaseModel<TEntity, TKey, TContext>
        where TContext : DbContext
        where TEntity : IEntityBase<TKey>, new()
    {
        /// <summary>
        /// Список идентификаторов сущностей
        /// </summary>
        TKey[] Ids { get; set; }

        IQueryable<TEntity> GetQuarable(TContext context);
    }
}
