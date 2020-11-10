using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class KickedOutsRepository : IKickedOutsRepository
    {
        private readonly ApplicationDbContext _context;

        public KickedOutsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<KickedOut> GetAll()
        {
            return _context.KickedOuts.ToList();
        }

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public async Task AddKickedUserAsync(string userId, string roomId, DateTime dateUnkick)
        {
            _context.KickedOuts.Add(
                new KickedOut
                {
                    UserId = userId,
                    RoomId = roomId,
                    DateUnkick = dateUnkick,
                });
            await SaveAsync();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public async Task DeleteKickedUserAsync(string userId, string roomId)
        {
            KickedOut kickedOut = _context.KickedOuts
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            if (kickedOut != null)
            {
                _context.Remove(kickedOut);
            }
            await SaveAsync();
        }

        /// <summary>
        /// Метод возвращает комнаты из которых выгнали пользователя
        /// </summary>
        public IEnumerable<KickedOut> GetListKickedRoomForUser(string userId)
        {
            return _context.KickedOuts
               .Where(k => k.UserId == userId)
               .ToList();
        }

        /// <summary>
        /// Метод возвращает коллекцию id комнат из которых выгнали пользователя
        /// </summary>
        public IEnumerable<string> GetListIdsKickedRoomForUser(string userId)
        {
            return _context.KickedOuts
               .Where(k => k.UserId == userId)
               .Select(k => k.RoomId)
               .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
