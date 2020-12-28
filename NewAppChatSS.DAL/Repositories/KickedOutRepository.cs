using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class KickedOutRepository : BaseRepository<KickedOut, int, ApplicationDbContext, KickedOutModel>, IKickedOutRepository
    {
        private readonly ApplicationDbContext context;

        public KickedOutRepository(ApplicationDbContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<List<KickedOut>> GetAllAsync()
        {
            var q = (IQueryable<KickedOut>)context.Set<KickedOut>();
            var m = context.Set<KickedOut>();
            return await q.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public async Task AddKickedUserAsync(string userId, string roomId, DateTime dateUnkick)
        {
            context.KickedOuts.Add(
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
            KickedOut kickedOut = context.KickedOuts
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            if (kickedOut != null)
            {
                context.Remove(kickedOut);
            }

            await SaveAsync();
        }

        /// <summary>
        /// Метод возвращает комнаты из которых выгнали пользователя
        /// </summary>
        public IEnumerable<KickedOut> GetListKickedRoomForUser(string userId)
        {
            return context.KickedOuts
               .Where(k => k.UserId == userId)
               .ToList();
        }

        /// <summary>
        /// Метод возвращает коллекцию id комнат из которых выгнали пользователя
        /// </summary>
        public IEnumerable<string> GetListIdsKickedRoomForUser(string userId)
        {
            return context.KickedOuts
               .Where(k => k.UserId == userId)
               .Select(k => k.RoomId)
               .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public Task<KickedOut> GetFirstOrDefaultAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task<KickedOut> GetLastOrDefaultAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetCountAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task<List<KickedOut>> GetAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(KickedOut row)
        {
            throw new NotImplementedException();
        }
    }
}
