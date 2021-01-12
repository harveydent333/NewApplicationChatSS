using System.Linq;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="Member"/>
    /// </summary>
    public class MemberModel : BaseModel<Member, int, NewAppChatSSContext>
    {
        /// <summary>
        /// Нужно ли возвращать пользователя <see cref="Member.User"/>
        /// </summary>
        public bool IncludeUser { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Нужно ли возвращать комнату <see cref="Member.Room"/>
        /// </summary>
        public bool IncludeRoom { get; set; }

        /// <summary>
        /// Идентификатор типа комнаты
        /// </summary>
        public string RoomId { get; set; }

        public override IQueryable<Member> GetQuarable(NewAppChatSSContext context)
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

        protected IQueryable<Member> AddUser(IQueryable<Member> query, bool includeUser)
        {
            if (includeUser)
            {
                query = query.Include(l => l.User);
            }

            return query;
        }

        protected IQueryable<Member> AddRoom(IQueryable<Member> query, bool includeRoom)
        {
            if (includeRoom)
            {
                query = query.Include(l => l.Room);
            }

            return query;
        }
    }
}