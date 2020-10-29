using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewAppChatSS.DAL.Repositories
{
    public class MutedUserRepository : IMutedUserRepository
    {
        private readonly ApplicationDbContext _context;

        public MutedUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает все записи пользователей лишенных права отправлять сообщения
        /// </summary>
        public IEnumerable<MutedUser> GetAll()
        {
            return _context.MutedUsers.ToList();
        }

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddMutedUser(string userId, string roomId, DateTime dateUnmute)
        {
            _context.MutedUsers.Add(
                new MutedUser
                {
                    UserId = userId,
                    RoomId = roomId,
                    DateUnmute = dateUnmute
                });
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteMutedUser(string userId, string roomId)
        {
            MutedUser mutedUser = _context.MutedUsers
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            if (mutedUser != null)
            {
                _context.Remove(mutedUser);
            }
            Save();
        }

        /// <summary>
        /// Метод возвращает время окончания ограничения права отправлять сообщения
        /// </summary>
        public DateTime GetDateTimeUnmuteUser(string userId, string roomId)
        {
            return _context.MutedUsers
                .FirstOrDefault(k => k.UserId == userId && k.RoomId == roomId)
                .DateUnmute;
        }

        /// <summary>
        /// Метод возвращает список комнаты где пользователь лишен права отправлять сообщения
        /// </summary>
        public IEnumerable<MutedUser> GetListMutedRoomForUser(string userId)
        {
            return _context.MutedUsers
             .Where(m => m.UserId == userId)
             .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}