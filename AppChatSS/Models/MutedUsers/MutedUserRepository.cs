using System;
using System.Collections.Generic;
using System.Linq;

namespace AppChatSS.Models.MutedUsers
{
    public class MutedUserRepository : IMutedUserRepository
    {
        private ApplicationDbContext mutedUserContext;

        public MutedUserRepository(ApplicationDbContext context)
        {
            mutedUserContext = context;
        }

        /// <summary>
        /// Возвращает коллекцию записей пользователей лишенных права отправлять сообщения
        /// </summary>
        public IQueryable<MutedUser> MutedUsers => mutedUserContext.MutedUsers;

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddMutedUser(Int32? userId, String roomId, DateTime dateUnblock)
        {
            mutedUserContext.MutedUsers.Add(new MutedUser
            {
                UserId = userId,
                RoomId = roomId,
                DateUnmute = dateUnblock
            });
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteMutedUser(Int32? userId, String roomId)
        {
            MutedUser mutedUser = mutedUserContext.MutedUsers
                .Where(m => m.UserId == userId && m.RoomId == roomId)
                .FirstOrDefault();

            if (mutedUser != null)
            {
                mutedUserContext.Remove(mutedUser);
            }

            Save();
        }

        /// <summary>
        /// Метод возвращает время окончания ограничения права отправлять сообщения
        /// </summary>
        public DateTime GetDateTimeUnmutedUser(Int32? userId, String roomId)
        {
            return mutedUserContext.MutedUsers
                .Where(k => k.UserId == userId && k.RoomId == roomId)
                .FirstOrDefault()
                .DateUnmute;
        }

        /// <summary>
        /// Метод возвращает список комнаты где пользователь лишен права отправлять сообщения
        /// </summary>
        public List<MutedUser> GetListMutedRoomForUser(Int32? userId)
        {
            return mutedUserContext.MutedUsers
                .Where(m => m.UserId == userId)
                .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public void Save()
        {
            mutedUserContext.SaveChanges();
        }
    }
}