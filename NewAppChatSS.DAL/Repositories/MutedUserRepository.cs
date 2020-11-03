﻿using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task AddMutedUserAsync(string userId, string roomId, DateTime dateUnmute)
        {
            _context.MutedUsers.Add(
                new MutedUser
                {
                    UserId = userId,
                    RoomId = roomId,
                    DateUnmute = dateUnmute
                });
            await SaveAsync();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public async Task DeleteMutedUserAsync(string userId, string roomId)
        {
            MutedUser mutedUser = _context.MutedUsers
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            if (mutedUser != null)
            {
                _context.Remove(mutedUser);
            }
            await SaveAsync();
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

        /// <summary>
        /// Метод возвращает коллекцию id комнат где пользователь лишен права отправлять сообщения
        /// </summary>
        public IEnumerable<string> GetListIdsMutedRoomForUser(string userId)
        {
            return _context.MutedUsers
                .Where(m => m.UserId == userId)
                .Select(k => k.RoomId)
                .ToList();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}