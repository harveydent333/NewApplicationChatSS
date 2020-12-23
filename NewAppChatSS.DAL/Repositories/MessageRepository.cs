using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext context;

        public MessageRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Метод возвращает все сообщения
        /// </summary>
        public IEnumerable<Message> GetAll()
        {
            return context.Messages.ToList();
        }

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public async Task AddMessage(Message item)
        {
            context.Messages.Add(item);
            await SaveAsync();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public async Task DeleteMessageAsync(string id)
        {
            context.Messages.Remove(
                context.Messages.FirstOrDefault(m => m.Id == id));

            await SaveAsync();
        }

        /// <summary>
        /// Метод ищет сообщения в комнате
        /// </summary>
        public IEnumerable<Message> FindMessagesByRoomId(string roomId)
        {
            return context.Messages
                .Where(m => m.RoomId == roomId)
                .ToList();
        }

        /// <summary>
        /// Метод возвращает сообщения комнаты
        /// </summary>
        public IEnumerable<Message> GetRoomMessages(string roomId)
        {
            return context.Messages
                .Include(m => m.User)
                .Include(m => m.Room)
                .Where(m => m.RoomId == roomId)
                .OrderBy(m => m.DatePublication)
                .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}