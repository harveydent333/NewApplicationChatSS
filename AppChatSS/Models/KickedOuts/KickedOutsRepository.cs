using System;
using System.Collections.Generic;
using System.Linq;

namespace AppChatSS.Models.KickedOuts
{
    public class KickedOutsRepository : IKickedOutsRepository
    {
        private ApplicationDbContext kickedOutContext;

        public KickedOutsRepository(ApplicationDbContext context)
        {
            kickedOutContext = context;
        }

        /// <summary>
        /// Возвращает коллекцию записей изгнанных пользователей
        /// </summary>
        public IQueryable<KickedOut> KickedOuts => kickedOutContext.KickedOuts;

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddKickeddUser(Int32? userId, String roomId, DateTime dateUnkick)
        {
            kickedOutContext.KickedOuts.Add(new KickedOut
            {
                UserId = userId,
                RoomId = roomId,
                DateUnkick = dateUnkick,
            });
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteKickedUser(Int32? userId, String roomId)
        {
            KickedOut kickedOut = kickedOutContext.KickedOuts
                .Where(m => m.UserId == userId && m.RoomId == roomId)
                .FirstOrDefault();
            if (kickedOut != null)
            {
                kickedOutContext.Remove(kickedOut);
            }
            Save();
        }

        /// <summary>
        /// Метод возвращает комнаты из которых выгнали пользователя
        /// </summary>
        public List<KickedOut> GetListKickedRoomForUser(Int32? userId)
        {
            return kickedOutContext.KickedOuts
                .Where(k => k.UserId == userId)
                .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public void Save()
        {
            kickedOutContext.SaveChanges();
        }
    }
}
