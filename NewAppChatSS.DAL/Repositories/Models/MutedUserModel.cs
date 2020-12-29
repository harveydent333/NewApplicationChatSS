using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="MutedUser"/>
    /// </summary>
    public class MutedUserModel : BaseModel<MutedUser, int, ApplicationDbContext>
    {
        /// <summary>
        /// Нужно ли возвращать пользователя <see cref="MutedUser.User"/>
        /// </summary>
        public bool IncludeUser { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Нужно ли возвращать комнату <see cref="MutedUser.Room"/>
        /// </summary>
        public bool IncludeRoom { get; set; }

        /// <summary>
        /// Идентификатор типа комнаты
        /// </summary>
        public string RoomId { get; set; }

        public override IQueryable<MutedUser> GetQuarable(ApplicationDbContext context)
        {
            var query = base.GetQuarable(context);

            if (UserId != null)
            {
                query = query.Where(q => q.UserId == UserId);
            }

            if (RoomId != null)
            {
                query = query.Where(q => q.RoomId == RoomId);
            }

            query = AddUser(query, IncludeUser);
            query = AddRoom(query, IncludeRoom);

            return query;
        }

        protected IQueryable<MutedUser> AddUser(IQueryable<MutedUser> query, bool includeUser)
        {
            if (includeUser)
            {
                query = query.Include(l => l.User);
            }

            return query;
        }

        protected IQueryable<MutedUser> AddRoom(IQueryable<MutedUser> query, bool includeRoom)
        {
            if (includeRoom)
            {
                query = query.Include(l => l.Room);
            }

            return query;
        }
    }
}