using System.Linq;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="Room"/>
    /// </summary>
    public class RoomModel : BaseModel<Room, string, NewAppChatSSContext>
    {
        /// <summary>
        /// Нужно ли возвращать владельца комнаты <see cref="Room.Owner"/>
        /// </summary>
        public bool IncludeOwner { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Нужно ли возвращать тип комнаты <see cref="Room.TypeRoom"/>
        /// </summary>
        public bool IncludeTypeRoom { get; set; }

        /// <summary>
        /// Идентификатор типа комнаты
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Нужно ли возвращать последнее сообщение в комнате <see cref="Room.LastMessage"/>
        /// </summary>
        public bool IncludeLastMessage { get; set; }

        /// <summary>
        /// Идентификатор последнего сообщения
        /// </summary>
        public string LastMessageId { get; set; }

        /// <summary>
        /// Имя комнаты
        /// </summary>
        public string RoomName { get; set; }

        public override IQueryable<Room> GetQuarable(NewAppChatSSContext context)
        {
            var query = base.GetQuarable(context);

            if (OwnerId != null)
            {
                query = query.Where(q => q.OwnerId == OwnerId);
            }

            if (TypeId.HasValue)
            {
                query = query.Where(q => q.TypeId == TypeId.Value);
            }

            if (LastMessageId != null)
            {
                query = query.Where(q => q.LastMessageId == LastMessageId);
            }

            if (RoomName != null)
            {
                query = query.Where(q => q.RoomName == RoomName);
            }

            query = AddOwner(query, IncludeOwner);
            query = AddTypeRoom(query, IncludeTypeRoom);
            query = AddLastMessage(query, IncludeLastMessage);

            return query;
        }

        protected IQueryable<Room> AddOwner(IQueryable<Room> query, bool includeOwner)
        {
            if (includeOwner)
            {
                query = query.Include(l => l.Owner);
            }

            return query;
        }

        protected IQueryable<Room> AddTypeRoom(IQueryable<Room> query, bool includeTypeRoom)
        {
            if (includeTypeRoom)
            {
                query = query.Include(l => l.TypeRoom);
            }

            return query;
        }

        protected IQueryable<Room> AddLastMessage(IQueryable<Room> query, bool includeLastMessage)
        {
            if (includeLastMessage)
            {
                query = query.Include(l => l.LastMessage);
            }

            return query;
        }
    }
}